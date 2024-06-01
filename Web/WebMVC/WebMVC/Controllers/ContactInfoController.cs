using Microsoft.AspNetCore.Mvc;
using WebMVC.Services.Interface;
using WebMVC.ViewModels.Common;
using WebMVC.ViewModels.ContactInfo;

namespace WebMVC.Controllers
{
    public class ContactInfoController : Controller
    {
        private IContactInfoService _contactInfoService;

        public ContactInfoController(IContactInfoService contactInfoService)
        {
            _contactInfoService = contactInfoService;
        }

        public IActionResult Index()
        {
            return View(new List<QueryRes>());
        }

        [HttpPost]
        public async Task<IActionResult> Index(QueryReq req)
        {
            var res = await _contactInfoService.QueryByConditionAsync(req);
            return View(res);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var res = await _contactInfoService.QueryByIdAsync(id ?? 0);
            return View(res);
        }

        public IActionResult Create()
        {
            return View(new CreateReq());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReq req)
        {
            if (ModelState.IsValid)
            {
                var res = await _contactInfoService.CreateAsync(req);
                if (res)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("CreateError", "新增失敗!");
                    return View(req);
                }
            }
            else
            {
                ModelState.AddModelError("CreateError", "新增失敗!");
                return View(req);
            }
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var res = await _contactInfoService.QueryByIdAsync(id ?? 0);
            return View(new EditReq
            {
                Id = res.ContactInfoID,
                Name = res.Name,
                Nickname = res.Nickname,
                Gender = res.Gender,
                Age = res.Age,
                PhoneNo = res.PhoneNo,
                Address = res.Address
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditReq req)
        {
            if (ModelState.IsValid)
            {
                var res = await _contactInfoService.EditAsync(req);
                if (res)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("EditError", "更新失敗!");
                    return View(req);
                }
            }
            else
            {
                ModelState.AddModelError("EditError", "更新失敗!");
                return View(req);
            }
        }

        public async Task<IActionResult> Remove(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var res = await _contactInfoService.QueryByIdAsync(id ?? 0);
            return View(res);
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromRoute]IdReq req)
        {
            var res = await _contactInfoService.RemoveAsync(new List<long> { req.Id ?? 0 });
            if (res)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("RemoveError", "刪除失敗!");
                return View();
            }
        }
    }
}
