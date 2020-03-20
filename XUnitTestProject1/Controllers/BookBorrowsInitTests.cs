using Library.Entities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class BookBorrowsInitTests
    {

        private readonly TestServer _server;
        private readonly HttpClient _client;

        public BookBorrowsInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.BookBorrow.Add(new BookBorrow
                {


                    IdBookBorrow = 200,
                    IdUser = 200,
                    IdBook = 200,
                    BorrowDate = new DateTime(2020, 3, 19),
                    ReturnDate = new DateTime(2020, 4, 3),
                    Comments = "Borrowed"


                });

                _db.SaveChanges();

            }
        }


        [Fact]
        public async Task PostBookBorrow()
        {

            var newBookBorrow = new BookBorrow
            {
                IdUser = 200,
                IdBookBorrow = 201,
                IdBook = 201,
                BorrowDate = new DateTime(2020, 3, 19),
                ReturnDate = new DateTime(2020, 4, 3),
                Comments = "Borrowed"
            };
            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows", new StringContent(
                    JsonConvert.SerializeObject(newBookBorrow),
                    Encoding.UTF8,
                    "application/json"
                ));

            httpResponse.EnsureSuccessStatusCode();

            var content = await httpResponse.Content.ReadAsStringAsync();
            var bookBorrow = JsonConvert.DeserializeObject<BookBorrow>(content);

            Assert.True(bookBorrow.IdBook == 201);
        }





        [Fact]
        public async Task PutBookBorrow()
        {
            int BookBorrowId = 200;
            string newComment = "Returned";

            var updatedBookBorrow = new BookBorrow
            {
                IdUser = 200,
                IdBookBorrow = BookBorrowId,
                IdBook = 201,
                BorrowDate = new DateTime(2020, 3, 19),
                ReturnDate = new DateTime(2020, 4, 3),
                Comments = newComment
            };
            var httpResponse = await _client.PutAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/{BookBorrowId}", new StringContent(
                    JsonConvert.SerializeObject(updatedBookBorrow),
                    Encoding.UTF8,
                    "application/json"
                ));

            httpResponse.EnsureSuccessStatusCode();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
                Assert.True(_db.BookBorrow.Any(e => e.IdBookBorrow == 200 && e.Comments == newComment));
            }


        }
    }
}
