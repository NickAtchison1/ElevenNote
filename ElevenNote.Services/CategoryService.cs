using ElevenNote.Data;
using ElevenNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services
{
    public class CategoryService
    {
        private readonly Guid _userId;
        public CategoryService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreateCategory(CategoryCreate model)
        {
            var entity = new Category()
            {
                OwnerId = _userId,
                Name = model.Name,

            };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Categories.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<CategoryListItem> GetCategories()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Categories
                    .Where(e => e.OwnerId == _userId)
                    .Select(e => new CategoryListItem()
                    {
                        CategoryId = e.Id,
                        Category = e.Name



                    });
                return query.ToList();
            }
        }

        public CategoryDetail GetCategoryById(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                Category entity = ctx.Categories.Include(e => e.Notes).Where(e => e.Id == id && e.OwnerId == _userId).Single();
                   // .Single(e => e.Id == id && e.OwnerId == _userId);
                return new CategoryDetail()
                {
                    CategoryId = entity.Id,
                    Name = entity.Name,
                    Notes = (List<NoteListItem>)entity.Notes.Select(e => new NoteListItem()
                    {
                        Title = e.Title
                    }).ToList(),
                    

                };

            }
        }

        public bool DeleteCategory(int categoryId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Categories.Single(e => e.Id == categoryId && e.OwnerId == _userId);
                ctx.Categories.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }
    }
}
