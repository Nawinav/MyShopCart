using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core;
using MyShop.Core.Models;
using MyShop.Core.ViewModel;
using MyShop.DataAccess.InMemory;

using MyShop.Services;
using MyShop.Core.Contracts;
using System.IO;

namespace MyShop.WebUI.Controllers
{
    public class ProductManagerController : Controller
    {
        // GET: ProductManager
        IRepository<Product> context;
        IRepository<ProductCategory> productCategories;
        public ProductManagerController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContext)
        {
            context= productContext;
            productCategories = productCategoryContext;
        }
        public ActionResult Index()
            
        {
            List<Product> products = context.Collection().ToList();
            return View(products);
        }
        public ActionResult Create()
        {
            ProductManagerViewModel ViewModel = new ProductManagerViewModel();
            ViewModel.Product = new Product();
            ViewModel.ProductCategories = productCategories.Collection(); 
            return View(ViewModel);
        }
        [HttpPost]
        public ActionResult Create(Product product,HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            else
            {
                if (file !=null)
                {
                    product.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//ProductImages//")+product.Image);
                }
                context.Insert(product);
                context.commit();
                return RedirectToAction("Index");
            }
        }
        public ActionResult Edit(string Id)
        {
            Product product = context.Find(Id);
            if(product==null){
                return HttpNotFound();
            }
            else
            {
                ProductManagerViewModel viewModel = new ProductManagerViewModel();
                viewModel.Product =product;
                viewModel.ProductCategories = productCategories.Collection();
                return View(viewModel);
            }
        }
        [HttpPost]
        public ActionResult Edit(Product product,string Id,HttpPostedFileBase file)
        {
            Product productToEdit = context.Find(Id);
            if (productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }

                if (file != null)
                {
                    productToEdit.Image = productToEdit.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//ProductImages//") + productToEdit.Image);
                }

                    productToEdit.Name = product.Name;
                    productToEdit.Category = product.Category;
                    productToEdit.Description = product.Description;
                    productToEdit.Price = product.Price;

                    context.commit();
                    return RedirectToAction("Index");
                
            }

        }
        public ActionResult Delete(string Id)
        {
            Product productToDelete = context.Find(Id);
            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productToDelete);
            }

        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            Product productToDelete = context.Find(Id);
            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                context.Delete(Id);
                return RedirectToAction("Index");
            }

        }

    }
}