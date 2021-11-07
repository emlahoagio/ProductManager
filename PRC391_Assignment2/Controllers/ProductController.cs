using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using PRC391_Assignment2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRC391_Assignment2.Controllers
{
    public class ProductController : Controller
    {
        private readonly IDynamoDBContext _dynamoDBContext;
        public ProductController(IDynamoDBContext dynamoDBContext)
        {
            _dynamoDBContext = dynamoDBContext;
        }
        public async Task<IActionResult> Index()
        {
            var condition = new List<ScanCondition>();
            var products = await _dynamoDBContext.ScanAsync<Product>(condition).GetRemainingAsync();
            return View(products);
        }
        #region Validation
        private async Task<bool> CheckDuplicated(Product p)
        {
            var condition = new List<ScanCondition>();
            var products = await _dynamoDBContext.ScanAsync<Product>(condition).GetRemainingAsync();
            //Check id duplicate and well formed
            foreach (Product pro in products)
            {
                if (pro.ProductId == p.ProductId)
                {
                    return false;
                }
            }
            return true;
        }
        private bool Validate(Product p)
        {
            //Check id well formed
            if (p.ProductId < 1 && p.ProductId > 999999999) return false;
            //Check Name Length > 5
            if (p.ProductName.Length < 6) return false;
            //Check Price > 0
            if (p.Price < 1) return false;
            //check Quan > 0
            if (p.Quantity < 1) return false;
            return true;
        }
        #endregion
        public IActionResult Error()
        {
            return View();
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (await CheckDuplicated(product) == false)
            {
                return RedirectToAction(nameof(Error));
            }
            if (Validate(product) == false)
            {
                return RedirectToAction(nameof(Error));
            }
            if (ModelState.IsValid)
            {
                await _dynamoDBContext.SaveAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        private async Task<Product> GetById(int id)
        {
            var condition = new List<ScanCondition>();
            var products = await _dynamoDBContext.ScanAsync<Product>(condition).GetRemainingAsync();
            foreach (Product p in products)
            {
                if (p.ProductId == id)
                {
                    return p;
                }
            }
            return null;
        }
        public async Task<IActionResult> Edit(int id)
        {
            var product = await GetById(id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (Validate(product) == false)
            {
                return RedirectToAction(nameof(Error));
            }
            if (ModelState.IsValid)
            {
                await _dynamoDBContext.SaveAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var product = await GetById(id);
            await _dynamoDBContext.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await GetById(id);
            return View(product);
        }
    }
}
