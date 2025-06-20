namespace Application.Utils
{
    public static class Constants
    {
        // Validaciones genéricas
        public const string RequiredField = "El campo {PropertyName} es obligatorio.";
        public const string MustBeBase64 = "El campo {PropertyName} debe ser una cadena base64 válida.";
        public const string InvalidLatitude = "La {PropertyName} debe estar entre -90 y 90.";
        public const string InvalidLongitude = "La {PropertyName} debe estar entre -180 y 180.";

        // Validaciones específicas del dominio
        public const string MustHaveAtLeastOneItem = "Debe agregar al menos un producto a la orden.";
        public const string InvalidDistanceRange = "La distancia debe estar entre 1 y 1000 kilómetros.";
        public const string CannotUpdateOrDelete = "Solo se permiten modificaciones si la orden está en estado 'Created'.";

        // Resultados de operaciones
        public const string OperationSuccess = "Operación completada exitosamente.";
        public const string OperationFailed = "Error al completar la operación.";
        public const string OrderCreated = "Orden creada correctamente.";
        public const string OrderUpdated = "Orden actualizada correctamente.";
        public const string OrderDeleted = "Orden eliminada correctamente.";
        public const string OrderRestored = "Orden restaurada correctamente.";
    }
}
