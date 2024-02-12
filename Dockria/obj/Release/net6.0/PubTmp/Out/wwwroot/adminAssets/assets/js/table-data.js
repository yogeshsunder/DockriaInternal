$(function(e) {
    "use strict";

    // Define an HTML structure for the search input with an icon
    var searchInputWithIcon = '<div class="input-group"><input type="text" class="form-control" placeholder="Search..."><div class="input-group-append"><span class="input-group-text"><i class="fa fa-search"></i></span></div></div>';


    //______Basic Data Table
    $('#basic-datatable').DataTable({
        language: {
            search: "", // Clear the default search text
        },
        // Replace the search input with the custom HTML structure
        initComplete: function () {
            var api = this.api();
            $(searchInputWithIcon)
                .appendTo($(api.table().container()).find('.dataTables_filter'))
                .on('keyup', function () {
                    api.search(this.value).draw();
                });
        },
    });

    //______Basic Data Table
    //$('#basic-datatable').DataTable({
    //    language: {
    //        searchPlaceholder: 'Search...',
    //        sSearch: '',
    //    }
    //});

        // ______Select2
    $(".select2").select2({
        minimumResultsForSearch: Infinity,
        width: '60px'
    });


    //______Basic Data Table
    $('#responsive-datatable').DataTable({
        language: {
            searchPlaceholder: 'Search...',
            scrollX: "100%",
            sSearch: '',
        }
    });

    //______File-Export Data Table
    var table = $('#file-datatable').DataTable({
        buttons: ['copy', 'excel', 'pdf', 'colvis'],
        language: {
            searchPlaceholder: 'Search...',
            scrollX: "100%",
            sSearch: '',
        }
    });
    table.buttons().container()
        .appendTo('#file-datatable_wrapper .col-md-6:eq(0)');

    //______Delete Data Table
    var table = $('#delete-datatable').DataTable({
        language: {
            searchPlaceholder: 'Search...',
            sSearch: '',
        }
    });
    $('#delete-datatable tbody').on('click', 'tr', function() {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        } else {
            table.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });
    $('#button').on('click', function() {
        table.row('.selected').remove().draw(false);
    });

    //______Responsive Modal Data Table
    $('#example3').DataTable( {
        language: {
            searchPlaceholder: 'Search...',
            sSearch: '',
        },
        responsive: {
            details: {
                display: $.fn.dataTable.Responsive.display.modal( {
                    header: function ( row ) {
                        var data = row.data();
                        return 'Details for '+data[0]+' '+data[1];
                    }
                } ),
                renderer: $.fn.dataTable.Responsive.renderer.tableAll( {
                    tableClass: 'table'
                } )
            }
        }
    } );
    // ______Select2
    $(".select2").select2({
        minimumResultsForSearch: Infinity,
        width: '60px'
    });

    //______Responsive Data Table
    $('#example2').DataTable({
		responsive: true,
		language: {
			searchPlaceholder: 'Search...',
			sSearch: '',
			lengthMenu: '_MENU_ items/page',
        },
        dom: 'l<"length-custom"i<"length-label">p>rt<"bottom"p>',
        initComplete: function () {
            // Calculate the number of rows in the table
            var table = this;
            var rowCount = table.rows().count();

            // Update the length label text
            $('.length-label').text('Total Rows: ' + rowCount);
        }
	});
        // ______Select2
    $(".select2").select2({
        minimumResultsForSearch: Infinity,
        width: '60px'
    });

});