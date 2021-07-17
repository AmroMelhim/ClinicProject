using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicProject.Data;
using ClinicProject.Models;

namespace ClinicProject.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public DoctorsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index(string order ,string SearchString,string CurrentFilter,int? PageNumber)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(order) ? "name_desc" : "";
            ViewData["NameSortParm2"] = order == "LastName" ? "LastName_desc" : "LastName";
            ViewData["AddressSortParm"] = order == "Address" ? "Address_desc" : "Address";
            ViewData["SalarySort"] = order == "MonthlySalary" ? "MonthlySalary_desc" : "MonthlySalary";
            ViewData["SpecializationSort"] = order == "Specialization" ? "Specialization_desc" : "Specialization";

            ViewData["CurrentSort"] = order;

            if (SearchString != null)
            {
                PageNumber = 1;
            }
            else
            {
                SearchString = CurrentFilter;
            }


            ViewData["CurrentFilter"] = SearchString;



            var applicationDBContext = from s in _context.Doctors.Include(p=>p.Specialization) select s;

            if (!String.IsNullOrEmpty(SearchString))
            {
                
                applicationDBContext = applicationDBContext.Where(s => s.FirstName.Contains(SearchString) || SearchString.Contains(s.LastName)
                || SearchString.Contains(s.FirstName));
            }



            switch (order)
            {
                case "name_desc":
                    applicationDBContext = applicationDBContext.OrderByDescending(s => s.FirstName);
                    break;
                case "LastName":
                    applicationDBContext = applicationDBContext.OrderBy(s => s.LastName);
                    break;
                case "LastName_desc":
                    applicationDBContext = applicationDBContext.OrderByDescending(s => s.LastName);
                    break;

                case "Address":
                    applicationDBContext = applicationDBContext.OrderBy(s => s.Address);
                    break;
                case "Address_desc":
                    applicationDBContext = applicationDBContext.OrderByDescending(s => s.Address);
                    break;
                case "MonthlySalary":
                    applicationDBContext = applicationDBContext.OrderBy(s => s.MonthlySalary);
                    break;
                case "MonthlySalary_desc":
                    applicationDBContext = applicationDBContext.OrderByDescending(s => s.MonthlySalary);
                    break;
                case "Specialization":
                    applicationDBContext = applicationDBContext.OrderBy(s => s.Specialization.SpecializationName);
                    break;
                case "Specialization_desc":
                    applicationDBContext = applicationDBContext.OrderByDescending(s => s.Specialization.SpecializationName);
                    break;

                default:
                    applicationDBContext = applicationDBContext.OrderBy(s => s.FirstName);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Doctor>.CreateAsync(applicationDBContext.AsNoTracking(), PageNumber ?? 1, pageSize));
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewData["SpecializationId"] = new SelectList(_context.Specializations, "Id", "SpecializationName");
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Address,Notes,MonthlySalary,PhoneNumber,Email,IBAN,SpecializationId")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecializationId"] = new SelectList(_context.Specializations, "SpecializationId", "SpecializationName");
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["SpecializationId"] = new SelectList(_context.Specializations, "Id", "Id", doctor.SpecializationId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FirstName,LastName,Address,Notes,MonthlySalary,PhoneNumber,Email,IBAN,SpecializationId")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecializationId"] = new SelectList(_context.Specializations, "Id", "Id", doctor.SpecializationId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(long id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> search(string FirstName)
        {
            
            var doctor = await _context.Doctors.Include(s=>s.Specialization).Where(n => n.FirstName == FirstName).FirstOrDefaultAsync();
            if(doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

    }
}
