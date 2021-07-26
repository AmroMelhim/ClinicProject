using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicProject.Data;
using ClinicProject.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ClinicProject.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public PatientsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index(string order, string SearchString, string CurrentFilter, int? PageNumber)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(order) ? "name_desc" : "";
            ViewData["NameSortParm2"] = order == "LastName" ? "LastName_desc" : "LastName";
            ViewData["AddressSortParm"] = order == "Address" ? "Address_desc" : "Address";
            ViewData["BirthDaySortParm"] = order == "BirthDay" ? "BirthDay_desc" : "BirthDay";
            ViewData["RegistrationSortParm"] = order == "Registration" ? "Registration_desc" : "Registration";
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



            var applicationDBContext = from s in _context.Patients select s;

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
                case "BirthDay":
                    applicationDBContext = applicationDBContext.OrderBy(s => s.BirthDay);
                    break;
                case "BirthDay_desc":
                    applicationDBContext = applicationDBContext.OrderByDescending(s => s.BirthDay);
                    break;
                case "Registration":
                    applicationDBContext = applicationDBContext.OrderBy(s => s.RegistrationDate);
                    break;
                case "Registration_desc":
                    applicationDBContext = applicationDBContext.OrderByDescending(s => s.RegistrationDate);
                    break;

                default:
                    applicationDBContext = applicationDBContext.OrderBy(s => s.FirstName);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Patient>.CreateAsync(applicationDBContext.AsNoTracking(), PageNumber ?? 1, pageSize));
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public async Task<IActionResult> CreateAsync()
        {
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,BirthDay,Gender,PhoneNumber,Email,Address,RegistrationDate,SSN,Country")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FirstName,LastName,BirthDay,Gender,PhoneNumber,Email,Address,RegistrationDate,SSN,Country")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
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
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");

            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(long id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }

        public async Task<IEnumerable<CountryModel>> GetCountry()
        {
            string temp = "https://restcountries.eu/rest/v2/all";
            List<CountryModel> country = new List<CountryModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(temp);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(temp);
                if (Res.IsSuccessStatusCode)
                {
                    var CountryResponse = Res.Content.ReadAsStringAsync().Result;
                    country = JsonConvert.DeserializeObject<List<CountryModel>>(CountryResponse);
                }
            }
            return country;

        }

    }
}
