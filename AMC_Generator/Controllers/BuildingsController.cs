using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMC_Generator.Models;
using AMC_Generator.Data;
using AMC_Generator.ModelViews;      // ← contains BuildingWithOwnerVM

namespace AMC_Generator.Controllers
{
    public class BuildingsController : Controller
    {
        private readonly AMC_GeneratorContext _ctx;
        public BuildingsController(AMC_GeneratorContext ctx) => _ctx = ctx;

        /* ---------- LIST ---------- */
        public async Task<IActionResult> Index()
            => View(await _ctx.Building.Include(b => b.Owner).ToListAsync());

        /* ---------- DETAILS ---------- */
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();
            var b = await _ctx.Building.Include(b => b.Owner)
                                       .FirstOrDefaultAsync(m => m.Id == id);
            return b is null ? NotFound() : View(b);
        }

        /* ---------- CREATE (combined Owner + Building) ---------- */
        public IActionResult Create() => View();   // returns BuildingWithOwnerVM empty model

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuildingWithOwnerVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // 1. Save Owner
            var owner = new Owner
            {
                FullName = vm.OwnerFullName,
                Phone = vm.OwnerPhone
            };
            _ctx.Owner.Add(owner);
            await _ctx.SaveChangesAsync();

            // 2. Save Building
            var building = new Building
            {
                Name = vm.BuildingName,
                Address = vm.BuildingAddress,
                OwnerId = owner.Id
            };
            _ctx.Building.Add(building);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /* ---------- EDIT ---------- */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();
            var b = await _ctx.Building.Include(x => x.Owner)
                                       .FirstOrDefaultAsync(x => x.Id == id);
            return b is null ? NotFound() : View(b);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Building building)
        {
            if (id != building.Id) return NotFound();

            if (!ModelState.IsValid) return View(building);

            try
            {
                _ctx.Update(building);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_ctx.Building.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        /* ---------- DELETE ---------- */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var b = await _ctx.Building.Include(x => x.Owner)
                                       .FirstOrDefaultAsync(m => m.Id == id);
            return b is null ? NotFound() : View(b);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var b = await _ctx.Building.FindAsync(id);
            if (b != null) _ctx.Building.Remove(b);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /* ---------- AJAX helper for AMC form ---------- */
        [HttpGet]
        public async Task<IActionResult> GetBuildingInfo(int id)
        {
            var b = await _ctx.Building.Include(x => x.Owner)
                                       .FirstOrDefaultAsync(x => x.Id == id);
            if (b is null) return NotFound();

            return Json(new
            {
                buildingName = b.Name,
                address = b.Address,
                ownerName = b.Owner.FullName,
                ownerPhone = b.Owner.Phone
            });
        }
        [HttpPost]
        //public async Task<IActionResult> QuickCreate(BuildingWithOwnerVM vm)
        //{
        //    if (!ModelState.IsValid) return BadRequest("بيانات غير صالحة");

        //    var owner = new Owner { FullName = vm.OwnerFullName, Phone = vm.OwnerPhone };
        //    _ctx.Owner.Add(owner);
        //    await _ctx.SaveChangesAsync();

        //    var building = new Building
        //    {
        //        Name = vm.BuildingName,
        //        Address = vm.BuildingAddress,
        //        OwnerId = owner.Id
        //    };
        //    _ctx.Building.Add(building);
        //    await _ctx.SaveChangesAsync();

        //    return Json(new
        //    {
        //        id = building.Id,
        //        name = building.Name,
        //        ownerName = owner.FullName,
        //        ownerPhone = owner.Phone,
        //        address = building.Address
        //    });
        //}
        [HttpPost]
        public async Task<IActionResult> QuickCreate(BuildingWithOwnerVM vm)
        {
            if (!ModelState.IsValid)
                return BadRequest("بيانات غير صالحة");

            var owner = new Owner
            {
                FullName = vm.OwnerFullName,
                PhoneCode = "+971",
                Phone = vm.OwnerPhone,
                City = vm.OwnerCity ?? string.Empty,
                StateOrEmirate = vm.OwnerState ?? string.Empty,
                Email = vm.OwnerEmail ?? string.Empty
            };
            _ctx.Owner.Add(owner);
            await _ctx.SaveChangesAsync();

            var building = new Building
            {
                Name = vm.BuildingName,
                Address = vm.BuildingAddress ?? string.Empty,
                OwnerId = owner.Id
            };
            _ctx.Building.Add(building);
            await _ctx.SaveChangesAsync();

            return Json(new
            {
                id = building.Id,
                name = building.Name,
                ownerName = owner.FullName,
                ownerPhone = owner.PhoneCode + " " + owner.Phone,
                address = building.Address
            });
        }


    }
}
