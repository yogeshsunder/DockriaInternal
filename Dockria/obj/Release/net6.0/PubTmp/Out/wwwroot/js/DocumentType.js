var dataTable = null;
// Check if the page is loaded for the first time
let firstLoad = true;
// Define a global variable to keep track of added metadata
var addedMetadata = [];
var metadataModalOpen = false;
$(document).ready(function () {
    const searchInput = $('#metadataSearchInput');
    const updateSearchInput = $('#updateMetaDataSearchInput');
    const searchIcon = $('#searchIcon');
    const updateSearchIcon = $('#updateSearchIcon');

    // Call the function to handle search input focus and blur
    handleSearchInputFocusBlur();
    // Add a focusin event listener to the input field
    searchInput.on('focusin', function () {
        debugger;
        searchIcon.css('display', 'none');
    });
    updateSearchInput.on('focusin', function () {
        debugger;
        updateSearchIcon.css('display', 'none');
    });

    // Add a focusout event listener to the input field
    searchInput.on('focusout', function () {
        debugger;
        searchIcon.css('display', 'block');
    });
    updateSearchInput.on('focusout', function () {
        debugger;
        updateSearchIcon.css('display', 'block');
    });

    // this function for load all details into Index view of Document Type....
    loadDataTable();

    // this is for show dropdown list showing of UserGrops
    fetchDropdownOptions('#groupList');

    // This is for show dropdown list showing of Auto File Names
    fetchAutoFileNameDropdown('#autoFileNameList');

    // this function for show Metadata PopUp view and Hide Add Document popUp view
    $('#metadata-modal').on('show.bs.modal', function (event) {
        debugger;
        var addDocumentModal = document.getElementById('addDocumentModal');
        var bootstrapAddDocumentModal = bootstrap.Modal.getInstance(addDocumentModal);
        bootstrapAddDocumentModal.hide();
        // Set the flag to true when metadata modal is opened
        metadataModalOpen = true;
        // Get selected metadata list from the metadata table
        var selectedMetadata = [];
        $(".Meta-Data tr").each(function () {
            var metadataName = $(this).find("td:eq(0)").text().trim(); // Get the metadataName from the first cell and trim any leading/trailing spaces
            var containerName = $(this).find("td:eq(2)").text().trim(); // Get the containerName from the third cell and trim any leading/trailing spaces
            var isChecked = $(this).find("input[type='checkbox']").prop("checked");

            //selectedMetadata.push({
            //    metadataName: metadataName,
            //    containerName: containerName,
            //    isRequired: isRequired
            //});
            selectedMetadata.push({ metadataName: metadataName, isRequired: isChecked, containerName: containerName });
        });
        fetchMetadata('metaDataTableBody', selectedMetadata);
    });

    // Handle search input changes of Add New Document Type MetaData List
    $("#metadataSearchInput").on("input", function () {
        // Fetch the search input value and filter metadata
        var searchValue = $(this).val().toLowerCase();
        filterMetadata('add', searchValue);
    });

    // Handle search input changes of Update Document Type MetaData List
    $("#updateMetaDataSearchInput").on("input", function () {
        // Fetch the search input value and filter metadata
        var searchValue = $(this).val().toLowerCase();
        filterMetadata('update', searchValue);
    });

    // this function for show UPDATE Case Metadata PopUp view and Hide Update Container Type popUp view
    $('#updateMetadata-modal').on('show.bs.modal', function (event) {
        var updateDocumentModal = document.getElementById('updateDocumentModal');
        var bootstrapUpdateDocumentModal = bootstrap.Modal.getInstance(updateDocumentModal);
        bootstrapUpdateDocumentModal.hide();
        debugger;
        var selectedMetadata = [];
        var rows = document.querySelectorAll('.Update-Meta-Data tr');
        rows.forEach(function (row) {
            debugger;
            var metadataName = row.querySelector('.col-8').textContent;
            var containerName = row.querySelector('.col-8').textContent;
            var isChecked = $(this).find("input[type='checkbox']").prop("checked");
            selectedMetadata.push({ metadataName: metadataName, isChecked: isChecked, containerName: containerName });
        });
        fetchMetadata('updateMetaDataTableBody', selectedMetadata);
    });

    // Add event listener to the "SAVE" button
    $(document).on('click', '#addUserSubmitButton', function () {
        debugger;
        //var selectedMetadata = getSelectedMetadata();
        saveDocument();
    });

    // Add event listener to the "UPDATE" button
    $(document).on('click', '#updateDocumentBtn', function () {
        debugger;
        updateDocument();
    });

    $('#metadata-modal').on('hidden.bs.modal', function () {
        // Reset the flag when metadata modal is closed
        metadataModalOpen = false;
        // Show the addDocumentTypeModal only if metadata modal is closed
        if (!metadataModalOpen) {
            showAddDocumentModal();
        }
    });

    $('#updateMetadata-modal').on('hidden.bs.modal', function () {
        showUpdateDocumentModal();
    });

    $('#addDocumentModal').on('hidden.bs.modal', function () {
        // Reset the addDocumentTypeModal only if metadata modal is not open
        if (!metadataModalOpen) {
            resetAddDocumentModal();
        }
    });

    // Add an event listener to the "ADD" button in the "Add Document" modal
    document.getElementById('metadataRequiredCheckbox').addEventListener('change', addMetadataToList);

});

// Function to handle search input focus and blur
function handleSearchInputFocusBlur() {
    $(document).on('focus', '.dataTables_wrapper input[type="search"]', function () {
        // Hide the icon when input is focused
        $(this).closest('.dataTables_wrapper').addClass('search-focused');
    });

    $(document).on('blur', '.dataTables_wrapper input[type="search"]', function () {
        // Show the icon when input loses focus
        $(this).closest('.dataTables_wrapper').removeClass('search-focused');
    });
}

function filterMetadata(context, searchValue) {
    debugger;
    var metaDataTableId = (context === 'add') ? '#metaDataTableBody' : '#updateMetaDataTableBody';
    var metadataRows = $(metaDataTableId).find("tr");
    metadataRows.each(function () {
        var metadataName = $(this).find("td:first").text().toLowerCase();
        if (metadataName.includes(searchValue)) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
}

// This Function for Show Add New Container Type PopUp View Modal...
function showAddDocumentModal() {
    debugger;
    var addDocumentModal = document.getElementById('addDocumentModal');
    var bootstrapAddDocumentModal = bootstrap.Modal.getInstance(addDocumentModal);
    bootstrapAddDocumentModal.show();
}

// This Function for Show Update Container Type PopUp View Modal...
function showUpdateDocumentModal() {
    debugger;
    var updateFileNameTypeModal = document.getElementById('updateDocumentModal');
    var bootStrapEditModal = bootstrap.Modal.getInstance(updateFileNameTypeModal);
    bootStrapEditModal.show();
}

// This Function for Show all Document Type data into Main Index view...
function loadDataTable() {
    debugger;
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/CompanyAdmin/Document/GetAll",
            "type": "GET",
            "dataType": "json",
            "error": function (xhr, status, error) {
                debugger;
                console.error("AJAX Error:", error);
            }
        },
        "columns": [
            {
                "data": "docTypeName",
                "width": "30%",
                "render": function (data) {
                    return '<div class="expandable-cell">' + data + '</div>';
                }
            },
            {
                "data": "ocr",
                "width": "10%",
                "render": function (data) {
                    if (data) {
                        return '<div class="expandable-cell">Enable</div>';
                    } else {
                        return '<div class="expandable-cell">Disable</div>';
                    }
                }
            },
            {
                "data": "autoFileName",
                "width": "40%",
                "render": function (data) {
                    return '<div class="expandable-cell">' + data + '</div>';
                }
            },
            {
                "data": "versioning",
                "width": "10%",
                "render": function (data) {
                    if (data) {
                        return '<div class="expandable-cell">Enable</div>';
                    } else {
                        return '<div class="expandable-cell">Disable</div>';
                    }
                }
            },
            {
                "data": "id",
                "width": "10%",
                "render": function (data, type, row) {
                    debugger;
                    var html = `<div class="form-button-action text-center">`;

                    html += `<button class="btn btn-link text-primary btn-simple-primary btn-lg btn-edit" data-toggle="tooltip" title="Edit Document Type"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="modal" data-bs-target="#editUserModal"`;
                    html += `data-user-id="${data}" data-action="edit" onclick=editDocumentType("/CompanyAdmin/Document/UpdateDocument/${data}")>`;
                    html += `<i class="fa fa-edit"></i></button>`;

                    html += `<button class="btn btn-link text-danger btn-simple-danger btn-delete" data-toggle="tooltip" title="Delete Document Type"`;
                    html += `data-original-title="Remove" data-bs-toggle="modal" data-bs-target="#deleteUserModal"`;
                    html += `data-user-id="${data}" data-action="delete" onclick=deleteDocumentType("/CompanyAdmin/Document/DeleteDocument/${data}")>`;
                    html += `<i class="fa fa-trash"></i></button>`;

                    html += `</div>`;
                    return html;
                }
            }
        ]
    })
}

// This Function for Delete Container Type Document...
function deleteDocumentType(url) {
    debugger;

    // Show a confirmation dialog using SweetAlert
    swal({
        title:' Do you want to Delete this Document Type MetaData?',
        
        icon: 'warning',
        buttons: {
            delete: {
                text: 'Delete',
                value: true,
                dangerMode: true,
            },
            cancel: 'Cancel',
        },
    }).then(function (confirmed) {
        if (confirmed) {
            // Perform the AJAX request
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (response) {
                    // Handle the response from the server
                    if (response) {
                        toastr.success(response.message);
                        // Reload the page after a delay of 2 seconds
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (error) {
                    // Handle AJAX error
                    // ...
                }
            });
        }
    });
}

// Function to get the current date and time in the format yyyy-mm-dd HH:mm:ss
function getCurrentDateTime() {
    debugger;
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');

    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

// Function to set the value of the input field and checkbox on first load
function setInitialValues() {
    debugger;
    if (firstLoad) {
        var metadataList = document.querySelector('.Meta-Data');
        var newRow = document.createElement('tr');
        newRow.setAttribute('data-metadata-name', getCurrentDateTime());
        newRow.innerHTML = `
                    <td class="col-8">
                        <input type="text" class="form-control no-border" >
                    </td>
                    <td class="col-2 text-center align-middle">
                        <label class="form-check-label d-flex justify-content-center align-items-center" style="height: 100%;">
                            <input type="checkbox" class="form-check-input">
                        </label>
                    </td>
                    <td class="col-1 text-center align-middle">
                        <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
                           data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'add')">
                           <i class="fa fa-trash"></i>
                        </a>
                    </td>
                    <td class="col-1 text-center align-middle">
                        <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add Metadata" data-bs-placement="top" data-bs-original-title="Add MetaData"
                        data-bs-toggle="modal" data-bs-target="#metadata-modal">
                        <i class="fa fa-plus-circle plus-icon"></i>
                        </a>
                    </td>
                `;
        metadataList.appendChild(newRow);

        firstLoad = false;
    }
}

// This function for Add MetaData List to the Document Type PopUp View
function addMetadata(context) {
    debugger;
    var metadataListSelector = (context === 'add') ? '.Meta-Data' : '.Update-Meta-Data';
    var metadataList = document.querySelector(metadataListSelector);
    var metaDataTableBodyId = (context === 'add') ? 'metaDataTableBody' : 'updateMetaDataTableBody';
    var metaDataModal = (context === 'add') ? 'metadata-modal' : 'updateMetadata-modal';

    // Get the selected metadata from the modal's checkboxes
    var selectedMetadata = [];
    var checkboxes = document.querySelectorAll(`#${metaDataTableBodyId} input[type="checkbox"]`);

    checkboxes.forEach(function (checkbox) {
        debugger;
        if (checkbox.checked) {
            var metadataName = checkbox.dataset.metadata;
            var container = checkbox.dataset.container;
            var isChecked = $(checkbox).prop("checked");
            selectedMetadata.push({ metadataName: metadataName, container: container, isChecked: isChecked });
        }
    });

    // Check if any metadata is selected
    if (selectedMetadata.length > 0) {
        // Handle selected metadata
        var addMetaData = [];
        if (context === 'add') {
            $(".Meta-Data tr").each(function () {
                debugger;
                var metadataName = $(this).attr("data-metadata-name");
                var container = $(this).attr("data-container-name");
                var isChecked = $(this).find("input[type='checkbox']").prop("checked");
                addMetaData.push({ metadataName: metadataName, container: container, isChecked: isChecked });
            });
        } else {
            var rows = document.querySelectorAll('.Update-Meta-Data tr');
            rows.forEach(function (row) {
                debugger;
                var metadataName = row.querySelector('.col-8').textContent;
                var isChecked = row.querySelector("input[type='checkbox']").checked;
                addMetaData.push({ metadataName: metadataName, isChecked: isChecked });
            });
            console.log('Add Metadata:', addMetaData);
        }
    } else {
        // No metadata selected, check if a container is selected
        var isContainerSelected = containerData.some(function (container) {
            debugger;
            return container.isSelected; // Assuming you have an 'isSelected' property
        });

        if (isContainerSelected) {
            debugger;
            // Display container names
            var containerNames = containerData.map(function (container) {
                debugger;
                return container.containerName;
            });
            console.log('Container Names:', containerNames);
        }
    }

    // Clear existing rows before populating
    metadataList.innerHTML = '';

    // Add selected metadata rows to the table
    // Function to show the custom alert modal
    function showAlert(message) {
        // Set the alert message
        $("#customAlertMessage").text(message);

        // Show the custom alert modal
        $("#customAlertModal").modal('show');
    }

    // Check the limit and add metadata rows
    if (selectedMetadata.length < 21) {
        selectedMetadata.forEach(function (metadataName) {
            debugger;
            addMetadataRow(metadataName, context, addMetaData);
            addedMetadata.push(metadataName); // Add to the addedMetadata array
            //numRowsAdded++;
        });
    } else {
        debugger;
        // Display a message in the custom alert modal when the maximum limit is reached
        showAlert("Maximum limit of 20 metadata rows has been reached.");
        return;
    }


    // Update Case
    if (context === 'update') {
        // Close the update-MetaData-modal after adding metadata
        var metadataModal = document.getElementById(metaDataModal);
        var bootstrapMetadataModal = bootstrap.Modal.getInstance(metadataModal);
        bootstrapMetadataModal.hide();

        // When the "Update-MetaData-modal" is hidden, show the "Update File Type Modal" modal
        $('#updateMetadata-modal').on('hidden.bs.modal', function (event) {
            debugger;
            showUpdateDocumentModal();
        });
    }

    // Add Case
    if (context === 'add') {
        // Close the metadata-modal after adding metadata
        var metadataModal = document.getElementById(metaDataModal);
        var bootstrapMetadataModal = bootstrap.Modal.getInstance(metadataModal);
        bootstrapMetadataModal.hide();
        // When the "Metadata Modal" is hidden, show the "Add New Document" modal
        $('#metadata-modal').on('hidden.bs.modal', function (event) {
            debugger;
            showAddDocumentModal();
        });
    }
}

function addMetadataRow(metadata, context, addMetaData) {
    debugger;
    var metaDataContext = (context === 'add') ? '.Meta-Data' : '.Update-Meta-Data';
    var metadataListTable = document.querySelector(metaDataContext);

    // Create the row
    var newRow = document.createElement('tr');
    newRow.setAttribute('data-metadata-name', metadata.metadataName);
    newRow.innerHTML = `
        <td class="col-8">${metadata.container || ''} ${metadata.metadataName || ''}</td>
        <td class="col-2 text-center align-middle">
            <label class="form-check-label d-flex justify-content-center align-items-center" style="height: 100%;">
                <input type="checkbox" class="form-check-input" ${metadata.isChecked ? 'checked' : ''}>
            </label>
        </td>
        <td class="col-1 text-center align-middle">
            <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
                data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'add')">
                <i class="fa fa-trash"></i>
            </a>
        </td>
        <td class="col-1 text-center align-middle">
            <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add Metadata" data-bs-placement="top" data-bs-original-title="Add MetaData"
                data-bs-toggle="modal" data-bs-target="#metadata-modal">
                <i class="fa fa-plus-circle plus-icon"></i>
            </a>
        </td>
    `;

    metadataListTable.appendChild(newRow);
}



// This function for Populate MetaData List to the Container Type PopUp View
//function addMetadataRow(metadata, context, addMetaData,container) {
//    debugger;
//    var metaDataContext = (context === 'add') ? '.Meta-Data' : '.Update-Meta-Data';
//    var metadataListTable = document.querySelector(metaDataContext);

//    var newRow = document.createElement('tr');

//    if (context === 'add') {
//        newRow.setAttribute('data-metadata-name', metadata.metadataName);
//      //  newRow.setAttribute('data-metadata-name', container.containerName);


//        debugger;
//        newRow.innerHTML = `
//            <td class="col-8">${metadata.metadataName}</td>
//            <td class="col-2 text-center align-middle">
//                <label class="form-check-label d-flex justify-content-center align-items-center" style="height: 100%;">
//                    <input type="checkbox" class="form-check-input" ${isCheckedInAddMetaData(metadata.metadataName, addMetaData) ? 'checked' : ''}>
//                </label>
//            </td>
//            <td class="col-1 text-center align-middle">
//                <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
//                    data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'add')">
//                    <i class="fa fa-trash"></i>
//                </a>
//            </td>
//            <td class="col-1 text-center align-middle">
//                <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add Metadata" data-bs-placement="top" data-bs-original-title="Add MetaData"
//                    data-bs-toggle="modal" data-bs-target="#metadata-modal">
//                    <i class="fa fa-plus-circle plus-icon"></i>
//                </a>
//            </td>
//        `;

//    } else {
//        newRow.setAttribute('update-metadata-name', metadata.metadataName);
//        newRow.setAttribute('update-container-name', containerName);
//        newRow.innerHTML = `
//            <td class="col-8">${metadata.metadataName}</td>
//            <td class="col-2 text-center align-middle">
//                <label class="form-check-label d-flex justify-content-center align-items-center" style="height: 100%;">
//                    <input type="checkbox" class="form-check-input" ${isCheckedInAddMetaData(metadata.metadataName, addMetaData) ? 'checked' : ''}>
//                </label>
//            </td>
//            <td class="col-1 text-center align-middle">
//                <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
//                    data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'update')">
//                    <i class="fa fa-trash"></i>
//                </a>
//            </td>
//            <td class="col-1 text-center align-middle">
//                <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add Metadata" data-bs-placement="top" data-bs-original-title="Add MetaData"
//                    data-bs-toggle="modal" data-bs-target="#updateMetadata-modal">
//                    <i class="fa fa-plus-circle plus-icon"></i>
//                </a>
//            </td>
//        `;
//    }

//    metadataListTable.appendChild(newRow);
//}

function isCheckedInAddMetaData(metadataName, addMetaData) {
    var foundMetadata = addMetaData.find(function (item) {
        return item.metadataName === metadataName;
    });

    return foundMetadata && foundMetadata.isChecked;
}


// This function for show UserGrops list in dropdown of Document popup view
function fetchDropdownOptions(selector, selectedOptions) {
    debugger;
    $.ajax({
        url: '/CompanyAdmin/Document/ShowUserList',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            debugger;
            console.log(data.userlist);
            populateDropdown(selector, data.userlist, selectedOptions);
        },
        error: function (error) {
            console.error(error.statusText);
        }
    });

    // This function for populate all userGroup list into dropdown of document popup view
    function populateDropdown(selector, options, selectedOptions) {
        debugger;
        var dropdown = $(selector);
        dropdown.empty();
        dropdown.append('<option disabled selected>Select an option</option>');

        // Populate options
        $.each(options, function (index, option) {
            debugger;
            dropdown.append($('<option>', {
                value: option.value,
                text: option.text
            }));
        });

        // If selectedOptions is provided, set the selected options
        if (selectedOptions) {
            debugger;
            for (var i = 0; i < dropdown[0].options.length; i++) { // Use 'dropdown' instead of 'userGroupsSelect'
                debugger;
                if (selectedOptions.includes(dropdown[0].options[i].value)) {
                    dropdown[0].options[i].selected = true;
                } else {
                    dropdown[0].options[i].selected = false;
                }
            }
            dropdown.val(selectedOptions.map(option => option.value));
        }

        // Refresh the Bootstrap Selectpicker, if applicable
        if (dropdown.hasClass('selectpicker')) {
            dropdown.selectpicker('refresh');
        }
    }
}

// This function for show AutoFileName list in dropdown of Document popup view
function fetchAutoFileNameDropdown(selector, selectedAutoFile) {
    debugger;
    $.ajax({
        url: '/CompanyAdmin/Document/ShowAutoFileName',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            debugger;
            console.log(data);
            populateAutoFileNameDropdown(selector, data, selectedAutoFile);
        },
        error: function (error) {
            console.error(error.statusText);
        }
    });
    function populateAutoFileNameDropdown(selector, options, selectedOption) {
        debugger;
        var dropdown = $(selector);
        dropdown.empty();
        dropdown.append('<option disabled selected>Select an option</option>');

        // Populate options
        $.each(options, function (index, option) {
            debugger;
            var optionElement = $('<option>', {
                value: option.value,
                text: option.text
            });

            dropdown.append(optionElement);
        });

        // Select the specified option if it exists
        if (selectedOption) {
            // Iterate through options to find a match
            $.each(options, function (index, option) {
                debugger;
                if (option.text === selectedOption.text) {
                    debugger;
                    dropdown.val(option.value);
                    return false; // Break the loop
                }
            });
        }
    }
}


function fetchMetadata(tableBodyId, selectedMetadata) {
    debugger;
    $.ajax({
        url: '/CompanyAdmin/Document/GetAllDocTypeMetadata',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            debugger;
            if (data.success === true) {
                var metaData = data.metaData; // Separate metadata array
                var containerData = data.containerData; // Separate container data array

                var tableBody = document.getElementById(tableBodyId);
                tableBody.innerHTML = ''; // Clear existing rows before populating

                // Display metadata items
                metaData.forEach(function (metadata) {
                    var row = document.createElement('tr');
                    var metadataCell = document.createElement('td'); // Create a cell for metadataName
                    var metadataCheckboxCell = document.createElement('td'); // Create a cell for the metadata checkbox

                    metadataCell.textContent = metadata.metadataName; // Populate metadataName

                    row.appendChild(metadataCell); // Add metadataCell to the row

                    // Create a checkbox for metadata
                    var isMetadata = true;
                    var metadataCheckboxDiv = createCheckboxDiv(metadata.metadataName, selectedMetadata, isMetadata);
                    metadataCheckboxCell.appendChild(metadataCheckboxDiv);

                    row.appendChild(metadataCheckboxCell); // Add metadataCheckboxCell to the row
                    tableBody.appendChild(row);
                });

                // Display container names below metadata list
                containerData.forEach(function (container) {
                    var row = document.createElement('tr');
                    var containerCell = document.createElement('td'); // Create a cell for containerName
                    var containerCheckboxCell = document.createElement('td'); // Create a cell for the container checkbox

                    containerCell.textContent = container.containerName; // Populate containerName

                    row.appendChild(containerCell); // Add containerCell to the row

                    // Create a checkbox for the container
                    var isMetadata = false;
                    var containerCheckboxDiv = createCheckboxDiv(container.containerName, selectedMetadata, isMetadata);
                    containerCheckboxCell.appendChild(containerCheckboxDiv);

                    row.appendChild(containerCheckboxCell); // Add containerCheckboxCell to the row
                    tableBody.appendChild(row);
                });
            }
        },

        error: function (error) {
            console.error(error.statusText);
        }
    });
}

function createCheckboxDiv(itemName, selectedItems, isMetadata) {
    var checkboxDiv = document.createElement('div');
    checkboxDiv.className = 'form-check text-center';

    var checkboxInput = document.createElement('input');
    checkboxInput.type = 'checkbox';
    checkboxInput.className = 'form-check-input';

    if (isMetadata) {
        checkboxInput.dataset.metadata = itemName;
    } else {
        checkboxInput.dataset.container = itemName;
    }

    // Check if the item is in selectedItems and has isRequired as true
    var isItemSelected = selectedItems.some(item => {
        if (isMetadata) {
            return item.metadataName === itemName && item.isRequired;
        } else {
            return item.containerName === itemName && item.isRequired;
        }
    });

    if (isItemSelected) {
        checkboxInput.checked = true;
    }

    checkboxDiv.appendChild(checkboxInput);
    return checkboxDiv;
}







// This function for show all table data into MetaData popUp view
//function fetchMetadata(tableBodyId, selectedMetadata) {
//    debugger;
//    $.ajax({
//        url: '/CompanyAdmin/Document/GetAllDocTypeMetadata',
//        type: 'GET',
//        dataType: 'json',
//        success: function (data) {
//            debugger;
//            if (data.success === true) {
//                var fileTypeMetaData = data.data;
//                var tableBody = document.getElementById(tableBodyId);
//                tableBody.innerHTML = ''; // Clear existing rows before populating

//                fileTypeMetaData.forEach(function (metadata) {
//                    debugger;
//                    var row = document.createElement('tr');
//                    var nameCell = document.createElement('td');
//                    var actionsCell = document.createElement('td');

//                    nameCell.textContent = metadata.metadataName;
//                    actionsCell.innerHTML = `
//                        <div class="form-check text-center">
//                        <input type="checkbox" class="form-check-input" data-metadata="${metadata.metadataName}" ${selectedMetadata.
//                            some(item => item.metadataName === metadata.metadataName) ? 'checked' : ''}>
//                        </div>
//                         `;

//                    row.appendChild(nameCell);
//                    row.appendChild(actionsCell);
//                    tableBody.appendChild(row);
//                });
//            }
//        },
//        error: function (error) {
//            console.error(error.statusText);
//        }
//    });
//}

// This function for remove metadata from list
function showAlert(message) {
    // Set the alert message
    $("#customAlertMessage").text(message);

    // Show the custom alert modal
    $("#customAlertModal").modal('show');
}

// This function for remove metadata from the list
function removeMetadata(button, context) {
    debugger;
    var row = button.closest('tr');
    var metadataListSelector = (context === 'add') ? '.Meta-Data' : '.Update-Meta-Data';
    var metadataList = document.querySelector(metadataListSelector);

    if (metadataList.children.length > 1) {
        var metadataName = row.querySelector('.col-8').textContent.trim();
        var indexToRemove = addedMetadata.indexOf(metadataName);
        if (indexToRemove !== -1) {
            addedMetadata.splice(indexToRemove, 1);
        }
        row.remove();
    } else {
        showAlert("Cannot remove the only metadata row.");
    }
}

// Example usage
// removeMetadata(myButtonElement, 'add');


// This function for Iport MetaData From CSV/Excel file....
function submitForm() {
    debugger;
    var fileInput = document.getElementById("metaDataFile");
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append("formFile", file);

    $.ajax({
        url: "/CompanyAdmin/Document/ImportMetaData",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            debugger;
            if (response.status) {
                debugger;
                // Hide the "Import MetaData" modal
                $('#importMetaDataModal').modal('hide');

                // Show the "Select Metadata" modal with updated data
                showMetadataModal();
            }
        }
    });
}

function saveDocument() {
    debugger;
    var form = $("#addDocumentForm");

    // Clear existing error messages
    $(".error-message").empty();
    var isValid = true;

    // Validation for Document Type Name
    var docTypeName = $("#docTypeName").val();
    if (!docTypeName) {
        $("#DocTypeNameError").text("Document Type Name is required.");
        isValid = false;
    }

    // Clear the Document Type Name error message when input changes
    $("#docTypeName").on("input", function () {
        $("#DocTypeNameError").empty();
    });

    // Validation for User Group
    var selectedUserGroups = $("#groupList option:selected:not([disabled])").length;
    if (selectedUserGroups === 0) {
        $("#UserGroupError").text("At least one User Group must be selected.");
        isValid = false;
    }

    // Clear the User Group error message when a new option is selected
    $("#groupList").on("change", function () {
        $("#UserGroupError").empty();
    });

    // Validation for AUTO FILE NAME
    var selectedAutoFileName = $("#autoFileNameList").val();
    if (!selectedAutoFileName) {
        $("#AutoFileNameError").text("AUTO FILE NAME is required.");
        isValid = false;
    }

    // Clear the AUTO FILE NAME error message when a new option is selected
    $("#autoFileNameList").on("change", function () {
        $("#AutoFileNameError").empty();
    });

    if (!isValid) {
        return; // Stop the process if validation fails
    }

    // Get the state of the checkboxes
    var ocrCheckbox = $("#ocr").is(':checked');
    var versioningCheckbox = $("#versioning").is(':checked');

    // Get selected user group list from multi-select dropdown
    var selectedUserGroups = [];
    $("#groupList option:selected").each(function () {
        selectedUserGroups.push($(this).val());
    });

    // Get selected metadata list from the metadata table
    var selectedMetadata = [];
    // Get selected metadata list from the table with checkboxes
    var selectedMetadata = [];

    // Iterate through the rows in the table
    $(".Meta-Data tr").each(function () {
        debugger;
        var metadataName = $(this).find("td:eq(0)").text().trim(); // Get the metadataName from the first cell and trim any leading/trailing spaces
        var containerName = $(this).find("td:eq(2)").text().trim(); // Get the containerName from the third cell and trim any leading/trailing spaces
        var isChecked = $(this).find("input[type='checkbox']").prop("checked");

        //selectedMetadata.push({
        //    metadataName: metadataName,
        //    containerName: containerName,
        //    isRequired: isRequired
        //});
        selectedMetadata.push({ metadataName: metadataName, isRequired: isChecked });
    });

    if (selectedMetadata.length == 0) {
        var currentDateTime = getCurrentDateTime();
        selectedMetadata.push({ name: currentDateTime, isChecked: true });
    }

    var formData = new FormData(form[0]);
    formData.append("SelectedUserGroups", JSON.stringify(selectedUserGroups));
    formData.append("SelectedMetadata", JSON.stringify(selectedMetadata));
    // Append the checkbox values to the FormData
    formData.append("OCR", ocrCheckbox);
    formData.append("VERSIONING", versioningCheckbox);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: formData,
        dataType: "json",
        contentType: false, // Important for sending FormData
        processData: false, // Important for sending FormData
        success: function (response) {
            console.log(response);
            debugger;
            if (response.success) {
                debugger;
                // Reset the form fields inside the #addDocumentModal
                resetAddDocumentModal();
                // Hide the modal
                $("#addDocumentModal").modal("hide");
                not1(response.message);
                dataTable.ajax.reload();
            } else {
                // Document save failed, handle the error (e.g., show an error message)
                console.error("Document save failed:", response.errorMessage);
            }
        },
        error: function (error) {
            console.error("An error occurred:", error);
        }
    });
}


// this function for showing user define alert message .....
function not1(message) {
    debugger;
    notif({
        type: "success",
        msg: message,
        position: "right"
    });
};

// Call the function to set the initial values on page load
setInitialValues();

// Function to reset the form fields inside the #addDocumentModal
function resetAddDocumentModal() {
    debugger;
    $('#docTypeName').val(''); // Reset the text input field
    $('#ocr').prop('checked', false); // Uncheck the OCR Checkbox
    $('#versioning').prop('checked', false); // Uncheck the OCR Checkbox
    $('#groupList').val([]); // Reset the multi-select dropdown
    $('#autoFileNameList').val(''); // Reset the single-select dropdown
    $('.Meta-Data').empty(); // Clear the metadata table

    // Reset the firstLoad variable to true
    firstLoad = true;

    setInitialValues();
}

// Function for Update DocumentType MetaData "GET" Method
function editDocumentType(url) {
    debugger;
    var modal = $('#updateDocumentModal');
    numRowsAdded = 0;
    // Clear the existing metadata rows
    clearMetadataListTable();
    // Perform the AJAX request
    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            debugger;
            // Handle the response from the server
            if (response.success) {
                debugger;
                var docTypeDoc = response.data;
                $('#id').val(docTypeDoc.data.id);
                // Populate the filename input field
                $('#docTypeName_1').val(docTypeDoc.data.docTypeName);
                $('#ocr_1').prop('checked', docTypeDoc.data.ocr);
                $('#versioning_1').prop('checked', docTypeDoc.data.versioning);
                // Assuming you have retrieved the user group names array from the response
                var selectedUserGroupNames = docTypeDoc.data.userGroups;
                // Fetch and populate the dropdown options for user groups
                fetchDropdownOptions('#userGroupNames', selectedUserGroupNames);

                var selectedAutoFileNames = docTypeDoc.data.autoFileName;
                fetchAutoFileNameDropdown('#autoFileNameList_1', selectedAutoFileNames[0]);

                // Populate the metadata list
                var metadataList = docTypeDoc.data.metaDataList;
                metadataList.forEach(function (metaData) {
                    debugger;
                    // Call a function to add metadata to the list
                    updateMetaDataRow(metaData);
                });

                // Show the modal
                modal.modal('show');
            } else {
                toastr.error(data.message);
            }
        },
        error: function (error) {
            debugger;
            alert(error);
            // Handle AJAX error
            // ...
        }
    });

    function clearMetadataListTable() {
        var metadataListTable = document.querySelector('.Update-Meta-Data');
        metadataListTable.innerHTML = '';
    }
}

// This function Show MetaData in Update Document Type Modal Pop Up View....
function updateMetaDataRow(metaData) {
    debugger;
    var metadataListTable = document.querySelector('.Update-Meta-Data');

    if (metadataListTable.children.length === 0) {
        // Clear existing rows before populating
        metadataListTable.innerHTML = '';
    }

    var newRow = document.createElement('tr');
    newRow.innerHTML = `
        <td class="col-8">${metaData.metaDataName || metaData.containerName}</td>
        <td class="col-2 text-center align-middle">
            <label class="form-check-label d-flex justify-content-center align-items-center" style="height: 100%;">
                <input type="checkbox" class="form-check-input" ${metaData.required ? 'checked' : ''}>
            </label>
        </td>
        <td class="col-1 text-center align-middle">
            <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
                data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'update')">
                <i class="fa fa-trash"></i>
            </a>
        </td>
        <td class="col-1 text-center align-middle">
            <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add Metadata" data-bs-placement="top"
                data-bs-original-title="Add MetaData" data-bs-toggle="modal" data-bs-target="#updateMetadata-modal">
                <i class="fa fa-plus-circle plus-icon"></i>
            </a>
        </td>
         `;

    metadataListTable.appendChild(newRow);
}

// This function for Update Document Type MetaData 'POST' Method....
function updateDocument() {
    debugger;
    var form = $("#updateDocumentForm");


    // Clear existing error messages
    $(".error-message").empty();
    var isValid = true;

    // Validation for Document Type Name
    var docTypeName = $("#docTypeName_1").val();
    if (!docTypeName) {
        $("#DocTypeNameError_1").text("Document Type Name is required.");
        isValid = false;
    }

    // Clear the Document Type Name error message when input changes
    $("#docTypeName_1").on("input", function () {
        $("#DocTypeNameError_1").empty();
    });

    // Validation for User Group
    var selectedUserGroups = $("#userGroupNames option:selected:not([disabled])").length;
    if (selectedUserGroups === 0) {
        $("#UserGroupError_1").text("At least one User Group must be selected.");
        isValid = false;
    }

    // Clear the User Group error message when a new option is selected
    $("#userGroupNames").on("change", function () {
        $("#UserGroupError_1").empty();
    });

    // Validation for AUTO FILE NAME
    var selectedAutoFileName = $("#autoFileNameList_1").val();
    if (!selectedAutoFileName) {
        $("#AutoFileNameError_1").text("AUTO FILE NAME is required.");
        isValid = false;
    }
    // Clear the AUTO FILE NAME error message when a new option is selected
    $("#autoFileNameList_1").on("change", function () {
        $("#AutoFileNameError_1").empty();
    });

    if (!isValid) {
        return; // Stop the process if validation fails
    }

    // Get the state of the checkboxes
    var ocrCheckbox = $("#ocr_1").is(':checked');
    var versioningCheckbox = $("#versioning_1").is(':checked');

    // Get selected user group list from multi-select dropdown
    var selectedUserGroups = [];
    $("#userGroupNames option:selected").each(function () {
        selectedUserGroups.push($(this).val());
    });

    // Get selected metadata list from the metadata table
    var selectedMetadata = [];
    // Iterate through each row in the metadata list
    var rows = document.querySelectorAll('.Update-Meta-Data tr');
    rows.forEach(function (row) {
        var metadataName = row.querySelector('.col-8').textContent;
        var containerName = row.querySelector('.col-8').textContent;
        var isChecked = row.querySelector("input[type='checkbox']").checked;
        selectedMetadata.push({ metadataName: metadataName, isRequired: isChecked, containerName: containerName });
    });

    // Check if no metadata is added, set default metadata name to current datetime
    if (selectedMetadata.length === 0) {
        var currentDateTime = getCurrentDateTime();
        selectedMetadata.push({ name: currentDateTime, isChecked: true });
    }


    var formData = new FormData(form[0]);
    formData.append("UpdatedUserGroups", JSON.stringify(selectedUserGroups));
    formData.append("UpdatedMetaData", JSON.stringify(selectedMetadata));
    // Append the checkbox values to the FormData
    formData.append("OCR", ocrCheckbox);
    formData.append("VERSIONING", versioningCheckbox);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: formData,
        dataType: "json",
        contentType: false, // Important for sending FormData
        processData: false, // Important for sending FormData
        success: function (response) {
            debugger;
            if (response.success) {
                debugger;
                //numRowsAdded = 0;
                var metadataListTable = document.querySelector('.Update-Meta-Data');
                metadataListTable.innerHTML = '';
                // Hide the modal
                $("#updateDocumentModal").modal("hide");
                not1(response.message);
                dataTable.ajax.reload();
            } else {
                // Document save failed, handle the error (e.g., show an error message)
                console.error("Document save failed:", response.errorMessage);
            }
        },
        error: function (error) {
            console.error("An error occurred:", error);
        }
    });
}

// Download All GropList in Excel Format...
function exportExcel() {
    debugger;
    window.location.href = '/CompanyAdmin/Document/ExportExcelDocument';
}

// Download All Document Metadata in PDF Format...
function exportPdf() {
    debugger;
    window.location.href = '/CompanyAdmin/Document/ExportPdfDocument';
}
