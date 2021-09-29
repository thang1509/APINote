using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Note.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NoteController : ControllerBase
    {
        public NoteController(ILogger<NoteController> logger, IWebHostEnvironment environment)
        {

        }

        [HttpGet]
        public IEnumerable<Note> Get([FromQuery] string id, string title)
        {
            var data = readdata();
            if (!string.IsNullOrEmpty(id))
                data = data.Where(x => x.Id.Equals(id)).ToList();

            if (!string.IsNullOrEmpty(title))
                data = data.Where(x => x.Title.Contains(title)).ToList();


            return data;
        }


        [HttpGet("{id}")]
        public Note Get(string id)
        {
            return readdata().FirstOrDefault(x => (!string.IsNullOrEmpty(id) && x.Id.Equals(id)));
        }

        [HttpPost]
        public bool Post(Note note)
        {
            note.Id = Guid.NewGuid().ToString();
            note.IsDeleted = false;
            var data = readdata();
            data.Add(note);
            writedata(data);
            return true;
        }

        [HttpPut("{id}")]
        public bool Put(Note note, string id)
        {
            var data = readdata();
            var tmp = data.FirstOrDefault(x => (!string.IsNullOrEmpty(id) && x.Id.Equals(id)));
            tmp.Title = note.Title;
            tmp.Content = note.Content;
            tmp.Order = note.Order;
            writedata(data);

            return true;
        }

        [HttpDelete("{id}")]
        public bool Delete(string id)
        {
            var data = readdata();
            var tmp = data.FirstOrDefault(x => (!string.IsNullOrEmpty(id) && x.Id.Equals(id)));
            tmp.IsDeleted = true;
            writedata(data);
            return true;
        }

        void writedata(List<Note> data)
        {
            string txtdata = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            System.IO.File.WriteAllText("data.txt", txtdata);
        }

        List<Note> readdata()
        {
            if (!System.IO.File.Exists("data.txt"))
            {
                string txtdata = Newtonsoft.Json.JsonConvert.SerializeObject(new List<Note>());
                System.IO.File.WriteAllText("data.txt", txtdata);
            }
            string data = System.IO.File.ReadAllText("data.txt");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Note>>(data);
        }
    }

    public class Note
    {
        public Note()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { set; get; }
        public string Title { set; get; }
        public string Content { set; get; }
        public bool IsDeleted { set; get; }
        public int Order { set; get; }
    }
}
