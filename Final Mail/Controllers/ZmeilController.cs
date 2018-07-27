using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using MailRecords;

namespace MailRecord.Controllers
{
    public class DataEmailsController : ApiController
    {
        readonly string userName = System.Configuration.ConfigurationManager.AppSettings["uName"];
        private BasicTablesEntities db = new BasicTablesEntities();

        // GET: api/DataEmails
        public IQueryable<DataEmail> GetDataEmails()
        {
            return db.DataEmails;
        }

        // GET: api/DataEmails/5
        [ResponseType(typeof(DataEmail))]
        public IHttpActionResult GetDataEmail(int id)
        {
            DataEmail dataEmail = db.DataEmails.Find(id);
            if (dataEmail == null)
            {
                return NotFound();
            }

            return Ok(dataEmail);
        }

        // PUT: api/DataEmails/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDataEmail(int id, DataEmail dataEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dataEmail.Id)
            {
                return BadRequest();
            }

            db.Entry(dataEmail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataEmailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DataEmails
        [ResponseType(typeof(DataEmail))]
        public IHttpActionResult PostDataEmail(DataEmail dataEmail)
        {
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(dataEmail.To));
            mailMessage.Subject = dataEmail.Subject;
            mailMessage.Body = dataEmail.Body;
            dataEmail.Date = DateTime.Now;
            dataEmail.From = userName;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.DataEmails.Add(dataEmail);
            smtpClient.Send(mailMessage);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dataEmail.Id }, dataEmail);
        }

        // DELETE: api/DataEmails/5
        [ResponseType(typeof(DataEmail))]
        public IHttpActionResult DeleteDataEmail(int id)
        {
            DataEmail dataEmail = db.DataEmails.Find(id);
            if (dataEmail == null)
            {
                return NotFound();
            }

            db.DataEmails.Remove(dataEmail);
            db.SaveChanges();

            return Ok(dataEmail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DataEmailExists(int id)
        {
            return db.DataEmails.Count(e => e.Id == id) > 0;
        }
    }
}
