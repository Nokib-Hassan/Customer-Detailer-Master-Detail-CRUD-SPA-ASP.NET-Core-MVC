using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N1285413.Models;

namespace N1285413.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CustomersController(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        //public async Task<IActionResult> Index()
        //{
        //    var count = await _context.Customers.MaxAsync(x => x.CustomerId);
        //    string id = Convert.ToString(count + 1);
        //    ViewBag.id = id;
        //    var customers=await _context.Customers.Include(c=>c.DeliveryAddresses).ToListAsync();
        //    return View(customers);

        public async Task<IActionResult> Index()
        {
            var count = 0;
            string id = "";
            if (count != 0)
            {
                count = await _context.Customers.MaxAsync(x => x.CustomerId);
                id = Convert.ToString(count + 1);
                ViewBag.id = id;
            }
            else
            {
                id = Convert.ToString(count + 1);
                ViewBag.id = id;
            }
            var customers = await _context.Customers.Include(c => c.DeliveryAddresses).ToListAsync();
            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _context.Customers
                                       .Include(c => c.DeliveryAddresses)
                                       .FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return Json(new
            {
                customerId = customer.CustomerId,
                name = customer.Name,
                address = customer.Address,
                customerType = customer.CustomerType,
                businessStart = customer.BusinessStart,
                phone = customer.Phone,
                email = customer.Email,
                creditDetails = customer.CreditDetails,
                photo = customer.Photo,
                deliveryAddresses = customer.DeliveryAddresses.Select(d => new
                {
                    address = d.Address,
                    contactPerson = d.ContactPerson,
                    phone = d.Phone
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Customer customer, IFormFile? photo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (photo != null && photo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Path.GetFileNameWithoutExtension(photo.FileName);
                    var extension = Path.GetExtension(photo.FileName);
                    var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks.ToString()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await photo.CopyToAsync(stream);

                    customer.Photo = uniqueFileName;
                }

                if (customer.CustomerId > 0)
                {
                    //for edit
                    var existingCustomer = await _context.Customers
                                   .Include(c => c.DeliveryAddresses)
                                   .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);

                    if (existingCustomer != null)
                    {
                        existingCustomer.Name = customer.Name;
                        existingCustomer.Email = customer.Email;
                        existingCustomer.Phone = customer.Phone;
                        existingCustomer.Address = customer.Address;
                        existingCustomer.CustomerType = customer.CustomerType;
                        existingCustomer.CreditDetails = customer.CreditDetails;
                        existingCustomer.BusinessStart = customer.BusinessStart;
                        existingCustomer.Photo = customer.Photo ?? existingCustomer.Photo;

                        _context.DeliveryAddresses.RemoveRange(existingCustomer.DeliveryAddresses);
                        foreach (var address in customer.DeliveryAddresses)
                        {
                            existingCustomer.DeliveryAddresses.Add(new DeliveryAddress
                            {
                                Address = address.Address,
                                ContactPerson = address.ContactPerson,
                                Phone = address.Phone
                            });
                        }
                        _context.Customers.Update(existingCustomer);
                    }
                }
                else
                {
                    //for insert

                    _context.Customers.Add(customer);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest("An error occured while saving data!!");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Json(new
            {
                redirectTo = Url.Action("Index", "Customers")
            });
        }
    }
}