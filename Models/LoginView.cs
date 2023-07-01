using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EXPEDITEE_REST.Models
{
public class LoginView
{
    [DataType(DataType.EmailAddress)]
    public string Mail { get; set; }

    [DataType(DataType.Password)]
    public string Clave { get; set; }
}

}