$(document).ready(function () {
    $('#facturasTable').DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
        },
        responsive: true,
        pageLength: 5,
        lengthMenu: [[5, 10, 25, 50], [5, 10, 25, 50]],
        order: [[2, 'desc']], // 🔥 ordena por FECHA (columna 2)
        columnDefs: [
            {
                targets: -1, // columna acciones
                orderable: false,
                searchable: false
            }
        ]
    });
});