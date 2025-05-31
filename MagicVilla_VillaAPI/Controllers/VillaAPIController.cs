using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogging _logging;
        private readonly ApplicationDbContext _context;

        public VillaAPIController(ApplicationDbContext context, ILogging logging)
        {
            _context = context;
            _logging = logging;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logging.Log("Fetching all villas", "info");
            return Ok(_context.Villas.ToList());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200,Type=typeof(VillaDTO))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]

        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                _logging.Log("Invalid villa ID provided", "error");
                return BadRequest();
            }
            var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                _logging.Log($"Villa with ID {id} not found", "warning");
                return NotFound();
            }
            _logging.Log($"Villa with ID {id} retrieved successfully", "info");
            return Ok(villa);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (_context.Villas.FirstOrDefault(v => v.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists!");
                return BadRequest(ModelState);
            }
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Villa villa = new Villa
            {
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                ImageUrl = villaDTO.ImageUrl,
                Details = villaDTO.Details,
                Rate = villaDTO.Rate,
                Amenity = villaDTO.Amenity,
                CreatedDate = DateTime.Now.Date
            };

            _context.Villas.Add(villa);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetVilla), new { id = villaDTO.Id }, villaDTO);
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _context.Villas.Remove(villa);
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPut("{id}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
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
            Villa villa=new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                ImageUrl = villaDTO.ImageUrl,
                Details = villaDTO.Details,
                Rate = villaDTO.Rate,
                Amenity = villaDTO.Amenity,
                UpdatedDate = DateTime.Now.Date
            };
            _context.Villas.Update(villa);
            _context.SaveChanges();
            return NoContent();
        }


        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> pathchVillaDTO)
        {
            if (pathchVillaDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = _context.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaDTO villaDTO = new VillaDTO
            {
                Id = villa.Id,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Sqft = villa.Sqft,
                ImageUrl = villa.ImageUrl,
                Details = villa.Details,
                Rate = villa.Rate,
                Amenity = villa.Amenity
            };
            pathchVillaDTO.ApplyTo(villaDTO, ModelState);
            villa = new Villa
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                ImageUrl = villaDTO.ImageUrl,
                Details = villaDTO.Details,
                Rate = villaDTO.Rate,
                Amenity = villaDTO.Amenity,
                UpdatedDate = DateTime.Now.Date
            };
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Villas.Update(villa);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
