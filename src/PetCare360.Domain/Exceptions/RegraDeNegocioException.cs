using System;
using System.Collections.Generic;
using System.Text;

namespace PetCare360.Domain.Exceptions
{
    public class RegraDeNegocioException : Exception
    {
        public RegraDeNegocioException(string mensagem) : base(mensagem) { }
    }
}
