using AutoMapper;
using GolbonWebRoad.Application.Features.Brands.Commands;
using GolbonWebRoad.Application.Features.Brands.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.Brands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    public class BrandsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BrandsController(IMediator mediator, IMapper mapper)
        {
            _mediator=mediator;
            _mapper=mapper;
        }
        public async Task<IActionResult> Index()
        {
            var brands = await _mediator.Send(new GetBrandsQuery());
            var viewModel = _mapper.Map<BrandViewModel>(brands);
            return View(viewModel);

        }
        public IActionResult Create()
        {
            return View(new CreateBrandViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBrandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var command = _mapper.Map<CreateBrandCommand>(model);
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {

            var brand = await _mediator.Send(new GetBrandByIdQuery { Id=id });
            if (brand==null) return NotFound();
            var viewModel = _mapper.Map<EditBrandViewModel>(brand);
            return View(viewModel);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBrandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var command = _mapper.Map<UpdateBrandCommand>(model);
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _mediator.Send(new GetBrandByIdQuery { Id=id });
            if (brand==null) return NotFound();
            var viewModel = _mapper.Map<DeleteBrandViewModel>(brand);
            return View(viewModel);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.Send(new DeleteBrandCommand { Id=id });
            return RedirectToAction(nameof(Index));
        }
    }
}
