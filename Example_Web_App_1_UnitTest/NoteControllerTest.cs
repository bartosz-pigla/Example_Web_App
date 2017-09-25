using Example_Web_App_1.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Example_Web_App_1.Helpers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net;
using Example_Web_App_1.Models;

namespace Example_Web_App_1_UnitTest
{
    public class NoteControllerTest
    {
        private static Account account = new Account { FirstName = "Alicja", LastName = "Abacka", Login = "alicja", Password = "alicja" };
        private static List<Note> notes = new List<Note>
        {
            new Note
            {
                Title="physics",
                Content="prepare for the exam",
                CreationTime=new TimeSpan(),
                Account=account
            },
            new Note
            {
                Title="wedding anniversary",
                Content="do the shopping",
                CreationTime=new TimeSpan(),
                Account=account
            },
            new Note
            {
                Title="title3",
                Content="content3",
                CreationTime=new TimeSpan(),
                Account=account
            }
        };
        public NoteControllerTest()
        {
            InitContext();
        }

        private EDBContext _edbContext;

        public void InitContext()
        {
            var builder = new DbContextOptionsBuilder<EDBContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               // don't raise the error warning us that the in memory db doesn't support transactions
               .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var context = new EDBContext(builder.Options);

            var allAccounts = (from a in context.Account select a).ToList();
            if (allAccounts.Count != 0)
            {
                context.RemoveRange(allAccounts);
            }

            context.SaveChanges();
            context.Account.Add(account);
            context.SaveChanges();

            var allNotes = (from n in context.Note select n).ToList();
            if (allNotes.Count != 0)
            {
                context.RemoveRange(allNotes);
            }

            context.SaveChanges();

            notes.ForEach(n => n.AccountId = account.AccountId);
            context.AddRange(notes);
            context.SaveChanges();

            _edbContext = context;
        }

        [Fact]
        public void GetFirstPageOfNotes()
        {
            List<Note> expectedNotes = new List<Note> { notes[0], notes[1] };
            var controller = new NoteController(_edbContext);
            List<Note> result = controller.Get(account.AccountId,new Pagination(1, 2)).ToList();

            Assert.True(expectedNotes.SequenceEqual(result));
        }
        [Fact]
        public void GetLastPageOfUsers()
        {
            List<Note> expectedNotes = new List<Note> { notes[2] };
            var controller = new NoteController(_edbContext);
            List<Note> result = controller.Get(account.AccountId, new Pagination(2, 2)).ToList();

            Assert.True(expectedNotes.SequenceEqual(result));
        }

        [Fact]
        public void PutExistingAccountWithExistingAccount()
        {
            Note note=new Note
            {
                NoteId = notes[0].NoteId,
                Title = "chemistry",
                Content = "prepare for the exam",
                CreationTime = new TimeSpan(),
                Account = account,
                AccountId=account.AccountId
            };

            var controller = new NoteController(_edbContext);
            controller.Put(note);

            Assert.True((from n in _edbContext.Note where n.NoteId==note.NoteId select n).FirstOrDefault().Title.Equals("chemistry"));
        }

        [Fact]
        public void PutExistingAccountWithUnxistingAccount()
        {
            Note note = new Note
            {
                NoteId = notes[0].NoteId,
                Title = "chemistry",
                Content = "prepare for the exam",
                CreationTime = new TimeSpan(),
                Account = new Account
                {
                    FirstName="asda"
                }
            };

            var controller = new NoteController(_edbContext);

            var exception = Assert.Throws<System.Web.Http.HttpResponseException>(() => controller.Put(note));
            Assert.Equal(exception.Response.StatusCode, HttpStatusCode.Conflict);
        }

        [Fact]
        public void PutUnexistingNoteWithExistingAccount()
        {
            Note note = new Note
            {
                NoteId = 11,
                Title = "sadsad",
                Account=account,
                AccountId=account.AccountId
            };

            var controller = new NoteController(_edbContext);

            var exception = Assert.Throws<System.Web.Http.HttpResponseException>(() => controller.Put(note));
            Assert.Equal(exception.Response.StatusCode, HttpStatusCode.NotFound);
        }

        [Fact]
        public void PutUnexistingNoteWithUnexistingAccount()
        {
            Note note = new Note
            {
                NoteId = 11,
                Title = "sadsad"
            };

            var controller = new NoteController(_edbContext);

            var exception = Assert.Throws<System.Web.Http.HttpResponseException>(() => controller.Put(note));
            Assert.Equal(exception.Response.StatusCode, HttpStatusCode.Conflict);
        }

        [Fact]
        public void DeleteExistingNote()
        {
            Note note = notes[1];

            var controller = new NoteController(_edbContext);
            controller.Delete(note.NoteId);

            Note result = (from n in _edbContext.Note where n.NoteId==note.NoteId select n).FirstOrDefault();

            Assert.Null(result);
        }

        [Fact]
        public void DeleteUnexistingNote()
        {
            Note note = new Note
            {
                NoteId=11,
               Title="sadsad"
            };

            var controller = new NoteController(_edbContext);

            var exception = Assert.Throws<System.Web.Http.HttpResponseException>(() => controller.Delete(note.NoteId));
            Assert.Equal(exception.Response.StatusCode, HttpStatusCode.NotFound);
        }
    }
}
