using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using ProductList.Models;

public class ItemsController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();


    public ActionResult Index()
        {
            var model = new ItemIndexViewModel
            {
                Items = db.Items.ToList(),
                ItemForm = new ItemViewModel()
            };
            return View(model);
        }

    //GET: Items
    //public ActionResult Index()
    //{

    //    return View(db.Items.ToList());
    //}

    // This action is used to fetch items for AJAX
    public ActionResult GetItems()
    {
        var items = db.Items.ToList();
        return PartialView("_ItemList", items);
    }
   
    // GET: Items/Create
    // GET: Item/Create
    public ActionResult Create()
    {
        return View("Create");
    }

    // POST: Item/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "PKey,Name,Description,Price")] ItemViewModel itemForm)
    {
         Item newItem = null;
        if (ModelState.IsValid)
        {
            newItem = new Item
            {
                PKey = 0,
                Name = itemForm.Name,
                Description = itemForm.Description,
                Price = itemForm.Price
            };
            db.Items.Add(newItem);
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true, item = newItem });
            }

            return RedirectToAction("Index");

        }
        if (Request.IsAjaxRequest())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            return Json(new { success = false, errors = errors });
        }


        return View("Index", newItem);
    }

    [HttpGet]
    public ActionResult GetItem(int id)
    {
        var items = db.Items.ToList();
        var item = items.FirstOrDefault(i => i.PKey == id);
        if (item != null)
        {
            return Json(new { success = true, item = item }, JsonRequestBehavior.AllowGet);
        }
        return Json(new { success = false, message = "Item not found" }, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(ItemViewModel itemForm)
    {
        if (ModelState.IsValid)
        {
            var existingItem = db.Items.ToList().FirstOrDefault(i => i.PKey == itemForm.PKey);
            if (existingItem != null)
            {
                existingItem.Name = itemForm.Name;
                existingItem.Description = itemForm.Description;
                existingItem.Price = itemForm.Price;

                db.Entry(existingItem).State = EntityState.Modified;
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true, item = itemForm });
                }

                return RedirectToAction("Index");
            }
        }
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return Json(new { success = false, errors = errors });
    }

    
    // GET: Item/Delete/5
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Item item = db.Items.Find(id);
        if (item == null)
        {
            return HttpNotFound();
        }
        return View("Delete", item);
    }

    // POST: Item/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> Delete(long id)
    {
        try
        {
            var item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }

            db.Items.Remove(item);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the exception details for debugging
            System.Diagnostics.Debug.WriteLine("Exception in Delete action: " + ex.Message);
            return Json(new { success = false, message = "Exception occurred: " + ex.Message });
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}
