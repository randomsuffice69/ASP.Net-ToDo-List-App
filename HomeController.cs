using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {

        private ToDoContext context;

        public HomeController(ToDoContext ctx) => context = ctx;

        public IActionResult Index(string ID)
        {
            var filters = new Filters(ID);
            ViewBag.Filters = filters;

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;

            IQueryable<ToDo> query = context.ToDos.Include(t => t.Category).Include(t => t.Status);

            if(filters.HasCategory)
            {
                query = query.Where(t => t.CategoryID == filters.CategoryID);
            }

            if (filters.HasStatus)
            {
                query = query.Where(t => t.StatusID == filters.StatusID);
            }

            if (filters.HasDue)
            {
                var today = DateTime.Today;
                if(filters.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filters.IsFuture)
                {
                    query = query.Where(t => t.DueDate > today);
                }
                else if (filters.IsToday)
                {
                    query = query.Where(t => t.DueDate == today);
                }
            }

            var tasks = query.OrderBy(t => t.DueDate).ToList();

            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();

            var task = new ToDo { StatusID = "open" };

            return View(task);
        }

        [HttpPost]
        public IActionResult Add(ToDo task)
        {
            //data validation
            if (ModelState.IsValid)
            {
                context.ToDos.Add(task);//adds task
                context.SaveChanges();//saves changes
                return RedirectToAction("Index");//redirects to home
            }
            else
            {   //sends to previous view and reloads viewbag
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.Statuses = context.Statuses.ToList();
                return View(task);
            }
        }


        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join('-', filter);
            return RedirectToAction("Index", new { ID = id });
        }

        //update individual todo item to mark it as closed
        [HttpPost]
        public IActionResult MarkComplete([FromRoute] string id, ToDo selected)//get url from route, bind todo object called selected
        {
            selected = context.ToDos.Find(selected.ID)!;//read from db

            if(selected != null)
            {
                selected.StatusID = "closed";
                context.SaveChanges();
            }

            return RedirectToAction("Index", new { ID =id });
        }

        [HttpPost]
        public IActionResult DeleteComplete(string id)
        {
            var toDelete = context.ToDos.Where(t => t.StatusID == "closed").ToList();//get all todos with a status of id

            foreach(var task in toDelete)//loop through them
            {
                context.ToDos.Remove(task); //remove them
            }
            context.SaveChanges();//save them

            return RedirectToAction("Index", new { ID = id });
        }
    }
}
