// Inicialización de DataTables para la tabla de clientes
$(document).ready(function () {
    var table = $('#clientesTable').DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
        },
        responsive: true,
        // Opciones de longitud de página: ahora incluye 5
        pageLength: 5,
        lengthMenu: [[5, 10, 25, 50], [5, 10, 25, 50]],
        order: [[0, 'asc']],
        columnDefs: [
            {
                targets: -1, // Última columna (Acciones)
                orderable: false,
                searchable: false
            }
        ]
    });

    // Puedes agregar eventos o botones personalizados aquí si lo necesitas
});
