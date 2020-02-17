using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BackendMitrais.Models;
using System.Web;
using System.Text.RegularExpressions;

namespace BackendMitrais.Controllers
{
    public class MemberController : ApiController
    {
        // GET: api/Member
        public IEnumerable<member> Get()
        {
            List<member> listMember = new List<member>();
            using(DbMitrais dbm = new DbMitrais())
            {
                listMember = dbm.members.ToList<member>();
            }
            return listMember;
        }

        [HttpPost]
        public IHttpActionResult PostMember(member memberInserted)
        {
            bool errorDetected = false;
            string errorMsg = "";
            member mInDB = new member();

            //------------------ check mobile number -------------------------------------------
            if (memberInserted.mobile_number.Length == 0)
            {
                errorMsg = "Please enter your mobile number!";
                errorDetected = true;
                goto SKIP;
            }

            string pattern = "\\+?([ -]?\\d+)+|\\(\\d+\\)([ -]\\d+)";
            Match indonesianNumber = Regex.Match(memberInserted.mobile_number, pattern);
            if (!indonesianNumber.Success)
            {
                errorMsg = "Please enter valid Indonesian phone number!";
                errorDetected = true;
                goto SKIP;
            }

            //------------------ check first name -------------------------------------------
            if (memberInserted.first_name.Length == 0)
            {
                errorMsg = "Please enter your first name!";
                errorDetected = true;
                goto SKIP;
            }

            //------------------ check last name -------------------------------------------
            if (memberInserted.last_name.Length == 0)
            {
                errorMsg = "Please enter your last name!";
                errorDetected = true;
                goto SKIP;
            }

            //------------------ check email -------------------------------------------
            if (memberInserted.email.Length == 0)
            {
                errorMsg = "Please enter your email!";
                errorDetected = true;
                goto SKIP;
            }

            //------------------ check duplicate mobile number and phone --------------------------
            using (DbMitrais dbm = new DbMitrais())
            {
                mInDB = dbm.members.Where(m => m.mobile_number == memberInserted.mobile_number).FirstOrDefault();
                if (mInDB != null)
                {
                    if (mInDB.mobile_number == memberInserted.mobile_number)
                    {
                        errorDetected = true;
                        errorMsg += "MOBILE NUMBER IS ALREADY REGISTERED!";
                        goto SKIP;
                    }
                }

                mInDB = dbm.members.Where(m => m.email == memberInserted.email).FirstOrDefault();
                if (mInDB != null)
                {
                    if (mInDB.email == memberInserted.email)
                    {
                        errorDetected = true;
                        errorMsg = "EMAIL IS ALREADY REGISTERED!";
                        goto SKIP;
                    }
                }
            }

            SKIP:
            if(!errorDetected)
            {
                using (DbMitrais dbM = new DbMitrais())
                {
                    dbM.members.Add(memberInserted);
                    dbM.SaveChanges();
                }
            }            
            return Ok(new {status=errorDetected,msg=errorMsg});
        }
    }
}
