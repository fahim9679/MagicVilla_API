using AutoMapper;
using Azure;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaRepository _dbVilla;
        private readonly ILogging _logging;
        private readonly IMapper _mapper;

        public VillaAPIController(ILogging logging, IMapper mapper, IVillaRepository dbVilla)
        {

            _logging = logging;
            _mapper = mapper;
            _dbVilla = dbVilla;
            this._response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logging.Log("Fetching all villas", "info");
                IEnumerable<Villa> villas = await _dbVilla.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villas);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;

        }
        [HttpGet("{id:int}",Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200,Type=typeof(VillaDTO))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]

        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logging.Log("Invalid villa ID provided", "error");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _logging.Log($"Villa with ID {id} not found", "warning");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _logging.Log($"Villa with ID {id} retrieved successfully", "info");
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO villaCreateDTO)
        {
            try
            {
                //if(!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                if (await _dbVilla.GetAsync(v => v.Name.ToLower() == villaCreateDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa already exists!");
                    return BadRequest(ModelState);
                }
                if (villaCreateDTO == null)
                {
                   
                    return BadRequest(villaCreateDTO);
                }
                //if (villaDTO.Id > 0)
                //{
                //    return StatusCode(StatusCodes.Status500InternalServerError);
                //}
                Villa villa = _mapper.Map<Villa>(villaCreateDTO);
                //Villa villa = new Villa
                //{
                //    Name = villaCreateDTO.Name,
                //    Occupancy = villaCreateDTO.Occupancy,
                //    Sqft = villaCreateDTO.Sqft,
                //    ImageUrl = villaCreateDTO.ImageUrl,
                //    Details = villaCreateDTO.Details,
                //    Rate = villaCreateDTO.Rate,
                //    Amenity = villaCreateDTO.Amenity,
                //    CreatedDate = DateTime.Now.Date
                //};

                await _dbVilla.CreateAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtAction(nameof(GetVilla), new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;

        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {

                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                await _dbVilla.RemoveAsync(villa);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpPut("{id}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaUpdateDTO)
        {
            try
            {
                if (villaUpdateDTO == null || id != villaUpdateDTO.Id)
                {
                    return BadRequest();
                }
                //var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
                //if (villa == null)
                //{
                //    return NotFound();
                //}
                //villa.Name = villaDTO.Name;
                //villa.Occupancy = villaDTO.Occupancy;
                //villa.Sqft = villaDTO.Sqft;
                Villa villa = _mapper.Map<Villa>(villaUpdateDTO);
                //Villa villa=new()
                //{
                //    Id = villaUpdateDTO.Id,
                //    Name = villaUpdateDTO.Name,
                //    Occupancy = villaUpdateDTO.Occupancy,
                //    Sqft = villaUpdateDTO.Sqft,
                //    ImageUrl = villaUpdateDTO.ImageUrl,
                //    Details = villaUpdateDTO.Details,
                //    Rate = villaUpdateDTO.Rate,
                //    Amenity = villaUpdateDTO.Amenity,
                //    UpdatedDate = DateTime.Now.Date
                //};
                await _dbVilla.UpdateAsync(villa);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }


        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> pathchVillaDTO)
        {
            try
            {
                if (pathchVillaDTO == null || id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);
                if (villa == null)
                {
                    return NotFound();
                }
                VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);
                //VillaUpdateDTO villaDTO = new VillaUpdateDTO
                //{
                //    Id = villa.Id,
                //    Name = villa.Name,
                //    Occupancy = villa.Occupancy,
                //    Sqft = villa.Sqft,
                //    ImageUrl = villa.ImageUrl,
                //    Details = villa.Details,
                //    Rate = villa.Rate,
                //    Amenity = villa.Amenity
                //};
                pathchVillaDTO.ApplyTo(villaUpdateDTO, ModelState);
                Villa model = _mapper.Map<Villa>(villaUpdateDTO);

                //villa = new Villa
                //{
                //    Id = villaUpdateDTO.Id,
                //    Name = villaUpdateDTO.Name,
                //    Occupancy = villaUpdateDTO.Occupancy,
                //    Sqft = villaUpdateDTO.Sqft,
                //    ImageUrl = villaUpdateDTO.ImageUrl,
                //    Details = villaUpdateDTO.Details,
                //    Rate = villaUpdateDTO.Rate,
                //    Amenity = villaUpdateDTO.Amenity,
                //    UpdatedDate = DateTime.Now.Date
                //};
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _dbVilla.UpdateAsync(model);

                return NoContent();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }
    }
}
