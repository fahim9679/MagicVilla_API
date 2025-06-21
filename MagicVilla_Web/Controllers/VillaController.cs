using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> list = new List<VillaDTO>();
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString());
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while fetching data.");
            }
            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO villaCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(villaCreateDTO);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa created successfully!";
                    return RedirectToAction(nameof(IndexVilla));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while creating the villa.");
                }
            }
            return View(villaCreateDTO);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateVilla(int VillaId)
        {
            var response = await _villaService.GetAsync<APIResponse>(VillaId);
            if (response != null && response.IsSuccess)
            {
                VillaDTO villaDTO = JsonConvert.DeserializeObject<VillaDTO>(response.Result.ToString());
                VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villaDTO);
                return View(villaUpdateDTO);
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while fetching the villa details.");
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO villaUpdateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(villaUpdateDTO);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa updated successfully!";
                    return RedirectToAction(nameof(IndexVilla));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while creating the villa.");
                }
            }
            return View(villaUpdateDTO);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteVilla(int VillaId)
        {
            var response = await _villaService.GetAsync<APIResponse>(VillaId);
            if (response != null && response.IsSuccess)
            {
                VillaDTO villaDTO = JsonConvert.DeserializeObject<VillaDTO>(response.Result.ToString());
                return View(villaDTO);
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while fetching the villa details.");
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVilla(VillaDTO villaDTO)
        {

            var response = await _villaService.DeleteAsync<APIResponse>(villaDTO.Id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa deleted successfully!";
                return RedirectToAction(nameof(IndexVilla));
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while creating the villa.");
            }

            return View(villaDTO);
        }
    }
}
