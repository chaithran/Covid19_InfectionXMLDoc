using Covid19_Infection.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using static Covid19_Infection.Models.Patient;

namespace Covid19_Infection.Controllers
{
    [RoutePrefix("api/patient")]
    public class PatientController : ApiController
    {
        string path = HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["DocumentPath"]);
        // GET: api/patient
        [HttpGet]
        public HttpResponseMessage Get()
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(path))                    
                doc.Load(path);
            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(doc.OuterXml, Encoding.UTF8, "application/xml");
            return response;
        }

        // GET: /api/patient/getrange?fromDate=2020-01-01T01%3A30&filterByState=1
        [HttpGet]
        [Route("getrange")]
        public IHttpActionResult Get(DateTime fromDate,StateType filterByState)
        {
            try
            {
                if (File.Exists(path))
                {
                    XDocument xDoc = XDocument.Load(path);
                    var stateVal = Enum.GetName(typeof(StateType), filterByState);
                    var rows = xDoc.Root.Document.Elements("Patients").Elements("Patient")
                            .Where(x => x.Attribute("State").Value == stateVal).ToList();
                    var result = rows.Where(x => DateTime.Parse(x.Attribute("IdentifiedOn").Value, CultureInfo.InvariantCulture) >= fromDate).ToList();
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return NotFound();
            }
        }

        // POST: /api/patient/createpatient
        [HttpPost]
        [Route("createpatient")]
        public void Post([FromBody]Patient patientModel)
        {
            XmlDocument xmlDocument = new XmlDocument();
            if (!File.Exists(path))
                CreateDocument();
            xmlDocument.Load(path);
            try
            {
                if (ModelState.IsValid)
                {
                    XmlElement xNewChild = xmlDocument.CreateElement("Patient");
                    var xDoc = XDocument.Load(path);
                    var count = xDoc.Descendants("Patient").Count();
                    xNewChild.SetAttribute("Id", (count++).ToString());
                    xNewChild.SetAttribute("Name", patientModel.Name);
                    xNewChild.SetAttribute("Age", patientModel.Age.ToString());
                    xNewChild.SetAttribute("Address", patientModel.Address);
                    xNewChild.SetAttribute("RelatedId", patientModel.RelatedId.ToString());
                    xNewChild.SetAttribute("State", patientModel.State.ToString());
                    xNewChild.SetAttribute("Criticality", patientModel.Criticality.ToString());
                    xNewChild.SetAttribute("IdentifiedOn", DateTime.Now.ToString());
                    xmlDocument.DocumentElement.AppendChild(xNewChild);
                    xmlDocument.Save(path);

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        // PUT: /api/patient/updatepatient?id=1&value=1
        [HttpPut]
        [Route("updatepatient")]
        public IHttpActionResult Put(int id, StateType value)
        {
            try
            {
                XDocument xmlDoc = XDocument.Load(path);
                var item = xmlDoc.Root.Document.Elements("Patients").Elements("Patient")
                            .Where(x => x.Attribute("Id").Value == id.ToString()).ToList().FirstOrDefault();
                if (item.HasAttributes)
                {
                    var stateVal = Enum.GetName(typeof(StateType), value);
                    item.Attribute("State").Value = stateVal;
                }
                xmlDoc.Save(path);
                return Ok();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return NotFound();
            }
        }

        private void CreateDocument()
        {            
            XDocument doc = new XDocument();
            doc.Add(new XElement("Patients"));
            doc.Save(path);
        }
    }
}
