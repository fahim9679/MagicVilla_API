using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _mapper = mapper;
            _villaService = villaService;
        }
        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> list = new List<VillaNumberDTO>();
            var response = await _villaNumberService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(response.Result.ToString());
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while fetching data.");
            }
            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberCreateVM = new();
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                villaNumberCreateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (response.Result.ToString()).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(villaNumberCreateVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa created successfully!";
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while creating the villa.");
                    
                }
            }
            var responseVill = await _villaService.GetAllAsync<APIResponse>();
            if (responseVill != null && responseVill.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (responseVill.Result.ToString()).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateVillaNumber(int VillaNo)
        {
            VillaNumberUpdateVM villaNumberUpdateVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(VillaNo);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(response.Result.ToString());
                villaNumberUpdateVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
                
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while fetching the villa details.");
            }
            response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                villaNumberUpdateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (response.Result.ToString()).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                return View(villaNumberUpdateVM);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Updated successfully!";
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while creating the villa.");

                }
            }
            var responseVill = await _villaService.GetAllAsync<APIResponse>();
            if (responseVill != null && responseVill.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (responseVill.Result.ToString()).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteVillaNumber(int VillaNo)
        {
            VillaNumberDeleteVM villaNumberDeleteVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(VillaNo);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(response.Result.ToString());
                villaNumberDeleteVM.VillaNumber = model;

            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while fetching the villa details.");
            }
            response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                villaNumberDeleteVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (response.Result.ToString()).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                return View(villaNumberDeleteVM);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {

            var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa deleted successfully!";
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage.FirstOrDefault() ?? "An error occurred while creating the villa.");
            }

            return View(model);
        }
    }
}
