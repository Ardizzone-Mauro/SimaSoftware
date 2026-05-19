//$(document).ready(function () {

//    $('#enviosTable').DataTable({

//        language: {
//            url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
//        },

//        responsive: true,

//        pageLength: 5,

//        lengthMenu: [
//            [5, 10, 25, 50],
//            [5, 10, 25, 50]
//        ],

//        order: [[0, 'desc']],

//        columnDefs: [
//            {
//                targets: -1,
//                orderable: false,
//                searchable: false
//            }
//        ]

//    });

//});

$(document).ready(function () {

    if ($.fn.DataTable.isDataTable('#enviosTable')) {
        $('#enviosTable').DataTable().destroy();
    }

    $('#enviosTable').DataTable({

        destroy: true,

        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
        },

        pageLength: 5,

        lengthMenu: [
            [5, 10, 25, 50],
            [5, 10, 25, 50]
        ],

        order: [[0, 'desc']],

        autoWidth: false,

        columnDefs: [
            {
                targets: 5,
                orderable: false,
                searchable: false
            }
        ]

    });

});