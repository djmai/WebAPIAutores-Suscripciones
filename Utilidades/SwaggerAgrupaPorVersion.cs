using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebAPIAutores.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceControlador = controller.ControllerType.Namespace; // Controller.V1
            var versionAPI = namespaceControlador.Split(".").Last().ToLower(); // v1

            controller.ApiExplorer.GroupName = versionAPI;

        }
    }
}