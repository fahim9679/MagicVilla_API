using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogging _logging;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext context, ILogging logging, IMapper mapper)
        {
            _context = context;
            _logging = logging;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logging.Log("Fetching all villas", "info");
            IEnumerable<Villa> villas = await _context.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villas));
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200,Type=typeof(VillaDTO))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]

        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                _logging.Log("Invalid villa ID provided", "error");
                return BadRequest();
            }
            var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                _logging.Log($"Villa with ID {id} not found", "warning");
                return NotFound();
            }
            _logging.Log($"Villa with ID {id} retrieved successfully", "info");
            return Ok(_mapper.Map<VillaDTO>(villa));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO villaCreateDTO)
        {
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (await _context.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == villaCreateDTO.Name.ToLower()) != null)
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
            Villa villa=_mapper.Map<Villa>(villaCreateDTO);
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

            await _context.Villas.AddAsync(villa);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVilla), new { id = villa.Id }, villa);
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa =await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _context.Villas.Remove(villa);
           await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaUpdateDTO)
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
            _context.Villas.Update(villa);
          await  _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> pathchVillaDTO)
        {
            if (pathchVillaDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _context.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
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
            _context.Villas.Update(model);
           await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
