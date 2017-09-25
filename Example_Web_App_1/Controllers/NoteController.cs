using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Example_Web_App_1.Helpers;
using System.Net;
using Example_Web_App_1.Models;
using Microsoft.AspNetCore.Authorization;

namespace Example_Web_App_1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class NoteController : Controller
    {
        public EDBContext DBContext { get; }
        public NoteController(EDBContext dBContext)
        {
            DBContext = dBContext;
        }

        //public NoteController()
        //{

        //}

        [HttpGet("{accountId}")]
        public IEnumerable<Note> Get(int accountId, [System.Web.Http.FromUri] Pagination pagination)
        {
            var accountNotes = from n in DBContext.Note where n.AccountId == accountId orderby n.CreationTime descending select n;

            return accountNotes.Skip((pagination.Number - 1) * pagination.Size).Take(pagination.Size);
        }

        [HttpGet("{noteId}")]
        public Note Get(int noteId)
        {
            var query =
                from p in DBContext.Note
                where p.NoteId == noteId
                select p;
            return query.FirstOrDefault();
        }

        [HttpPost]
        public void Post([FromBody]Note newNote)
        {
            using (var transaction = DBContext.Database.BeginTransaction())
            {
                var checkIfAccountAlreadyExistsQuery =
                    from a in DBContext.Account
                    where a.AccountId == newNote.AccountId
                    select a;

                if (checkIfAccountAlreadyExistsQuery.FirstOrDefault() != null)
                {
                    newNote.CreationTime = DateTime.Now;
                    DBContext.Note.Add(newNote);
                    DBContext.SaveChanges();
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                    throw new System.Web.Http.HttpResponseException(HttpStatusCode.Conflict);
                }
            }
        }

        [HttpPut]
        public void Put([FromBody]Note updatedNote)
        {
            using (var transaction = DBContext.Database.BeginTransaction())
            {
                var checkIfAccountAlreadyExistsQuery =
                    from a in DBContext.Account
                    where a.AccountId == updatedNote.AccountId
                    select a;

                Account noteAccount = checkIfAccountAlreadyExistsQuery.FirstOrDefault();

                if (noteAccount == null)
                {
                    transaction.Rollback();
                    throw new System.Web.Http.HttpResponseException(HttpStatusCode.Conflict);
                }

                var checkIfNoteAlreadyExistsQuery =
                    from n in DBContext.Note
                    where n.NoteId==updatedNote.NoteId
                    select n;

                Note note = checkIfNoteAlreadyExistsQuery.FirstOrDefault();

                if (note != null)
                {
                    note.Title = updatedNote.Title;
                    note.Content = updatedNote.Content;

                    DBContext.Note.Update(note);
                    DBContext.SaveChanges();
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                    throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);
                }
            }
        }

        [HttpDelete("{noteId}")]
        public void Delete(int noteId)
        {
            using (var transaction = DBContext.Database.BeginTransaction())
            {
                var checkIfNoteAlreadyExistsQuery =
                    from n in DBContext.Note
                    where n.NoteId==noteId
                    select n;

                Note noteToDelete = checkIfNoteAlreadyExistsQuery.FirstOrDefault();

                if (noteToDelete != null)
                {
                    DBContext.Note.Remove(noteToDelete);
                    DBContext.SaveChanges();
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                    throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);
                }
            }
        }
    }
}
