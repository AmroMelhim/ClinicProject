using ClinicProject.Data;
using ClinicProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicProject.Controllers
{
    public class TestController : Controller
    {
        private readonly ApplicationDBContext _db;

        public TestController(ApplicationDBContext _context)
        {
            _db = _context;
        }
        public IActionResult Index()
        {
            var specialization = _db.Specializations.ToList();
            return View(specialization);
        }

        public async Task<IActionResult> Detail(long? id)
        {
            if (id == null)
            {
                return NotFound();            
            }
            var Specialization =await _db.Specializations.FirstOrDefaultAsync(i => i.Id == id);
            if (Specialization == null)
            {
                return NotFound();
            }
            return View(Specialization);
        }


    public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var Specialization = await _db.Specializations.FindAsync(id);
            return View(Specialization);

        }
        [HttpPost]
        public IActionResult Edit(Specialization specialization)
        {
            _db.Specializations.Update(specialization);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }
        public IActionResult Delete(long id)
        {
            var Specliazation = _db.Specializations.Find(id);
            if(Specliazation == null)
            {
                return NotFound();
            }
            _db.Specializations.Remove(Specliazation);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Specialization specialization)
        {
            _db.Specializations.Add(specialization);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
