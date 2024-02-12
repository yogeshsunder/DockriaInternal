var metaDataArray = [];
var rowCounter = 0;
var dataTable = null;
$(document).ready(function () {
    debugger;

    loadDataTable();
    $('#addFolderModal').on('show.bs.modal', function (event) {
        // Generate a unique random text string
        var uniqueName = generateUniqueRandomText();

        // Set the generated unique name in the input field
        $('#folderViewName').val(uniqueName);
        // Clear any existing rows (if any)
        $('#tableBody').empty();
        createAndAddNewRow('tableBody');
    });

    $('#updateFolderModal').on('show.bs.modal', function (event) {
        // Generate a unique random text string
        var uniqueName = generateUniqueRandomText();
        // Set the generated unique name in the input field
        $('#folderViewName').val(uniqueName);
    })

    // Use event delegation to handle "Add New Row" for Add case button click
    $("#tableBody").on("click", ".addRow", function () {
        debugger;
        createAndAddNewRow('tableBody');
    });

    // Use event delegation to handle "Add New Row" for Update Case button click
    $("#updateTableBody").on("click", ".addRow", function () {
        debugger;
        UpdateEachRowData();
    });

    // You can also handle the delete button click if needed
    // Function to show the custom alert modal
    function showAlert(message) {
        // Set the alert message
        $("#customAlertMessage").text(message);

        // Show the custom alert modal
        $("#customAlertModal").modal('show');
    }

    // Delete button click handler for addFolderModal
    $("#addFolderModal").on("click", ".btn-delete", function () {
        debugger;
        // Get the current row count
        var rowCount = $("#tableBody tr").length;
        // Check if there are more than 1 rows before removing
        if (rowCount > 1) {
            // Remove the row when the delete button is clicked
            $(this).closest("tr").remove();
        } else {
            // Display a message in the custom alert modal if there's only one row
            showAlert("Cannot delete the last row.");
        }
    });

    // Delete button click handler for updateFolderModal
    $("#updateFolderModal").on("click", ".btn-delete", function () {
        debugger;
        // Get the current row count
        var rowCount = $("#updateTableBody tr").length;
        // Check if there are more than 1 rows before removing
        if (rowCount > 1) {
            // Remove the row when the delete button is clicked
            $(this).closest("tr").remove();
        } else {
            // Display a message in the custom alert modal if there's only one row
            showAlert("Cannot delete the last row.");
        }
    });

    // Add event listener to the "SAVE" button
    $(document).on('click', '#addFolderSubmitButton', function () {
        debugger;
        //var selectedMetadata = getSelectedMetadata();
        saveFolderView();
    });

    // Add event listener to the "SAVE" button
    $(document).on('click', '#updateFolderSubmitBtn', function () {
        debugger;
        //var selectedMetadata = getSelectedMetadata();
        updateBtn();
    });

    handleSearchInputFocusBlur()
})

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

// Function to generate a unique random text string
function generateUniqueRandomText() {
    var textLength = 8; // Adjust the desired length of the random string
    var charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var uniqueText = "";

    for (var i = 0; i < textLength; i++) {
        var randomIndex = Math.floor(Math.random() * charset.length);
        uniqueText += charset.charAt(randomIndex);
    }

    return uniqueText;
}

// Define an object to store the options for the second dropdown.
var secondDropdownOptions = {
    "Document Type": [], // Initialize as an empty array
    "Container Type": [] // Initialize as an empty array
};

// Define an object to store dropdown options
var metaDataDropdownOptions = {
    "MetaData List": []
};

// Create cache objects for the second and third dropdowns
var secondDropdownOptionsCache = {};
var metaDataDropdownOptionsCache = {};

// Function to populate the second dropdown based on the selected value
function populateDropdown(dropDown, Value, rowCounter) {
    debugger;
    if (dropDown == "DocumentType") {
        var secondDropdown = document.getElementById(`objectType_${rowCounter}`); // Use the rowCounter to target the correct second dropdown
    } else if (dropDown == "MetaDataDropdown") {
        // Use the appropriate dropdown element based on your naming convention
        var metaDataDropdown = document.getElementById(`metaDataList_${rowCounter}`);
    }


    // Clear the options of the dropdown.
    if (dropDown == "DocumentType") {
        secondDropdown.innerHTML = "";
    } else if (dropDown == "MetaDataDropdown") {
        metaDataDropdown.innerHTML = "";
    }

    // Populate the options of the dropdown based on the selected value.
    if (dropDown == "DocumentType") {
        secondDropdownOptionsCache[Value].forEach(function (optionText) {
            debugger;
            var option = document.createElement("option");
            option.textContent = optionText;
            secondDropdown.appendChild(option);
        });
    } else if (dropDown == "MetaDataDropdown") {
        metaDataDropdownOptionsCache[Value].forEach(function (optionText) {
            debugger;
            var option = document.createElement("option");
            option.textContent = optionText;
            metaDataDropdown.appendChild(option);
        });
    }
}

// Function to create and add a new row
function createAndAddNewRow(selector, isUpdateCase = false, data = null) {
    debugger;
    var tableBody = document.getElementById(selector);

    rowCounter++;
    // Create a new table row
    var newRow = document.createElement("tr");
    newRow.id = "row_" + rowCounter;

    // Create and append table cells (td elements) to the row
    newRow.innerHTML = `
        <td class="text-center align-middle">
            <div class="select-with-arrow">
                <select for="SelectObject" name="SelectObject" class="form-control form-control-sm dropdown-toggle"
                        data-bs-toggle="dropdown" aria-required="true" id="selectObject_${rowCounter}">
                        <option style="display: none;" disabled selected></option>  
                                    </select>
            </div>
        </td>
        <td class="text-center align-middle">
            <div class="select-with-arrow">
                <select for="ObjectType" name="ObjectType" class="form-control form-control-sm dropdown-toggle"
                        data-bs-toggle="dropdown" aria-required="true" id="objectType_${rowCounter}">
                    <option disabled selected></option>  
                </select>
            </div>
        </td>
        <td class="text-center align-middle">
            <div class="select-with-arrow">
                <select for="MetaDataList" name="MetaDataList" class="form-control form-control-sm dropdown-toggle"
                        data-bs-toggle="dropdown" aria-required="true" id="metaDataList_${rowCounter}">
                    <option disabled selected></option>                    
                </select>
            </div>
        </td>
        <td class="text-center align-middle">
         <div class="select-with-arrow">
    <select for="Operator" name="Operator" class="form-control form-control-sm dropdown-toggle"
            data-bs-toggle="dropdown" aria-required="true" id="operator_${rowCounter}" onchange="toggleEntryValue(${rowCounter})">
        <option disabled selected></option>
        <option value="text">Text</option>
        <option value="calendar">Calendar</option>
    </select>
  </div>
        </td>
        <td class="text-center align-middle">
            <!-- Add the calendar input with a unique ID -->
            <input type="date" name="CalendarInput" id="calendarInput_${rowCounter}" class="form-control form-control-sm d-none" />
            <!-- Add the text input with a unique ID -->
               <input type="text" name="EntryValue" id="entryValueInput_${rowCounter}" placeholder="ENTER VALUE (Text Field or Calendar Value)" class="form-control form-control-sm" disabled />
        </td>
        <td id="rowId_${rowCounter}" class="text-center align-middle" style="display: none;">
            <input type="hidden" id="Id" value="${isUpdateCase ? data.id : '0'}">
        </td>
        <td class="text-center">
            <button type="button" class="btn btn-md btn-link text-success btn-add addRow" title="Add New Row" data-bs-placement="top">
                <i class="fa fa-plus-circle plus-icon"></i>
            </button>
            <button type="button" class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Delete Row" data-bs-placement="top">
                <i class="fa fa-trash"></i>
            </button>
        </td>
    `;

    // Append the new row to the table body
    tableBody.appendChild(newRow);

    var firstDropdown = newRow.querySelector(`#selectObject_${rowCounter}`);
    var secondDropdown = newRow.querySelector(`#objectType_${rowCounter}`);
    var metaDataDropdown = newRow.querySelector(`#metaDataList_${rowCounter}`);
    var operatorDropdown = newRow.querySelector(`#operator_${rowCounter}`);

    var dateSelectedOperator = ["=", "!=", ">", ">=", "<", "<=", "is empty", "is not empty", "With Date", "Without Date"];
    var selectedOperator = ["=", "!=", ">", ">=", "<", "<=", "Contains", "Does not Contain", "Starts With", "Does not Start with", "is empty", "is not empty"];

    var firstDropdownOptions = [
        { text: "", value: "" }, // Blank option

        { text: "Document Type", value: "Document Type" },
        { text: "Container Type", value: "Container Type" }
    ];

    firstDropdownOptions.forEach(function (option, index) {
        var optionElement = document.createElement("option");
        optionElement.textContent = option.text;
        optionElement.value = option.value;

        // Select "Document Type" by default
        if (index === 0) {
            optionElement.selected = true;
        }

        firstDropdown.appendChild(optionElement);
    });


    // Clear the second and third dropdowns initially
    secondDropdown.innerHTML = "";
    metaDataDropdown.innerHTML = "";

    firstDropdown.addEventListener("change", function () {
        debugger;
        var selectedValue = firstDropdown.value;

        // Clear the second and third dropdowns
        secondDropdown.innerHTML = "";
        metaDataDropdown.innerHTML = "";

        // Check if options for the selected value have already been loaded
        if (!secondDropdownOptionsCache[selectedValue]) {
            // Make an AJAX request to the server to fetch the options
            fetch(`/CompanyAdmin/FolderView/GetSecondDropdownValue?selectedValue=${selectedValue}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Network response was not ok: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    debugger;
                    if (data.status) {
                        debugger;
                        secondDropdownOptionsCache[selectedValue] = data.data.map(item => item.containerTypeName || item.docTypeName);
                        // Populate the second dropdown based on the selected value
                        populateDropdown("DocumentType", selectedValue, rowCounter);
                    } else {
                        // Handle the case where the server returns an error or no data
                        console.error(data.message);
                    }
                })
                .catch(error => console.error(error));
        } else {
            // Populate the second dropdown based on the selected value (if options already loaded)
            populateDropdown("DocumentType", selectedValue, rowCounter);
        }
    });

    secondDropdown.addEventListener("change", function () {
        debugger;
        var selectedValue = firstDropdown.value;
        var selectedObject = secondDropdown.value;

        // Clear the third dropdown
        metaDataDropdown.innerHTML = "";

        // Check if options for the selected value have already been loaded
        if (!metaDataDropdownOptionsCache[selectedObject]) {
            // Make an AJAX request to the server to fetch the options
            fetch(`/CompanyAdmin/FolderView/GetMetaDataDropdownValue?selectedValue=${selectedValue}&selectedObject=${selectedObject}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Network response was not ok: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    debugger;
                    if (data.status) {
                        debugger;
                        if (selectedValue == "Document Type") {
                            metaDataDropdownOptionsCache[selectedObject] = data.data.map(item => item.metaDataName);
                        }
                        else {
                            metaDataDropdownOptionsCache[selectedObject] = data.data.map(item => item.metadataName);
                        }

                        // Populate the second dropdown based on the selected value
                        populateDropdown("MetaDataDropdown", selectedObject, rowCounter);
                    } else {
                        // Handle the case where the server returns an error or no data
                        console.error(data.message);
                    }
                })
                .catch(error => console.error(error));
        } else {
            // Populate the second dropdown based on the selected value (if options already loaded)
            populateDropdown("MetaDataDropdown", selectedObject, rowCounter);
        }
    })

    metaDataDropdown.addEventListener("change", function () {
        debugger;
        var selectedValue = metaDataDropdown.value;
        var calendarInput = document.querySelector(`#calendarInput_${rowCounter}`);
        var entryValueInput = document.querySelector(`#entryValueInput_${rowCounter}`);

        // Check the selected metadata and decide whether to show the calendar input or text input
        if (selectedValue === "Current Date") {
            // Show the calendar input
            calendarInput.style.display = "block";
            entryValueInput.style.display = "none";
        } else {
            // Show the text input
            calendarInput.style.display = "none";
            entryValueInput.style.display = "block";
        }

        // Clear the operator dropdown initially
        operatorDropdown.innerHTML = "";

        // Check the selected metadata and populate the operator dropdown accordingly
        if (selectedValue === "Current Date") {
            // Populate with date operators
            dateSelectedOperator.forEach(function (operator) {
                var option = document.createElement("option");
                option.textContent = operator;
                option.value = operator;
                operatorDropdown.appendChild(option);
            });
        } else {
            // Populate with general operators
            selectedOperator.forEach(function (operator) {
                var option = document.createElement("option");
                option.textContent = operator;
                option.value = operator;
                operatorDropdown.appendChild(option);
            });
        }
    })


    // If it's an update case and data is provided, set the EntryValue
    if (isUpdateCase && data) {
        var updateFirstDropdown = row.querySelector(`[name='SelectObject']`);
        var updateSecondDropdown = row.querySelector(`[name='ObjectType']`);
        var updateMetaDataDropdown = row.querySelector(`[name='MetaDataList']`);
        var updateOperatorDropdown = row.querySelector(`[name='Operator']`);

        updateFirstDropdown.value = data.selectObject;
        updateSecondDropdown.value = data.objectType;
        updateMetaDataDropdown.value = data.metaDataList;
        updateOperatorDropdown.value = data.operators;

        var entryValueField = newRow.querySelector("[name='EntryValue']");
        entryValueField.value = data.value;
    }
}

// Function to populate the Metadata dropdown options for the initial row
function populateMetaDataOptionsForInitialRow() {
    debugger;
    var optionsHTML = populateMetaDataOptions(); // Call the existing function
    $("#metaDataList_1").html(optionsHTML); // Assuming the ID of the first row is metaDataList_1
}
function toggleEntryValue(rowCounter) {
    console.log('Toggle Entry Value Called for row: ' + rowCounter);

    var operatorSelect = document.getElementById('operator_' + rowCounter);
    var entryValueInput = document.getElementById('entryValueInput_' + rowCounter);

    // Enable/disable the input based on the selected operator
    entryValueInput.disabled = operatorSelect.value === '';
}

document.getElementById('operator_${rowCounter}').addEventListener('change', function () {
    var inputText = document.getElementById('entryValueInput_${rowCounter}');
    inputText.disabled = this.value === ''; // Disable if no option is selected
});

// This function for Add Folder View "POST" Method...
function saveFolderView() {
    debugger;
    var form = $("#FolderAddForm");

    // Create an array to store row data
    var rowsData = [];

    for (var i = 1; i <= rowCounter; i++) {
        var rowId = "row_" + i; // Generate the row's ID
        var row = document.getElementById(rowId); // Get the row element

        // Check if the row exists
        if (row) {
            // Extract data from the row using the row's ID
            var selectObjectValue = row.querySelector(`#selectObject_${i}`).value;
            var objectTypeValue = row.querySelector(`#objectType_${i}`).value;
            var metaDataListId = `metaDataList_${i}`;
            var metaDataListValue = row.querySelector(`#${metaDataListId}`).value;
            var operatorValue = row.querySelector(`#operator_${i}`).value;
            var entryValue = row.querySelector("[name='EntryValue']").value;
            var calendarValue = row.querySelector("[name='CalendarInput']").value;

            if (calendarValue) { entryValue = calendarValue }

            // Create an object to store row data
            var rowData = {
                SelectObject: selectObjectValue,
                ObjectType: objectTypeValue,
                MetaDataList: metaDataListValue,
                Operator: operatorValue,
                Value: entryValue
            };
            // Add the row data to the array
            rowsData.push(rowData);
        } else {
            console.error("Row not found:", rowId);
        }
    }

    // Create a FolderView object
    var folderViewData = {
        FolderViewName: $("#folderViewName").val(), // You might need to adjust the selector
        RowsData: rowsData
    };



    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: JSON.stringify(folderViewData),
        contentType: "application/json",
        success: function (response) {
            debugger;
            if (response.success) {
                debugger;
                // Reset the form fields inside the #addDocumentModal

                // Hide the modal
                $("#addFolderModal").modal("hide");
                not1(response.message);
                debugger;
                dataTable.ajax.reload();
            } else {
                if (response.isExistContainerNameError) {
                    // Display the error message on your view
                    $("#containerNameError").text(response.message);
                } else {
                    // Use SweetAlert for other error scenarios
                    swal({
                        title: 'Error',
                        text: 'An error occurred: ' + response.message,
                        icon: 'error'
                    });
                }
            }
        },
        error: function (error) {
            debugger;
            // Use SweetAlert for other error scenarios
            swal({
                title: 'Error',
                text: 'An error occurred: ' + error.message,
                icon: 'error'
            });
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

// this function for show all data into index main page...
function loadDataTable() {
    debugger;
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/CompanyAdmin/FolderView/GetAll",
            "type": "GET",
            "dataType": "json",
            "error": function (xhr, status, error) {
                debugger;
                console.error("AJAX Error:", error);
            }
        },
        "columns": [
            {
                "data": "folderViewName",
                "render": function (data) {
                    return '<div class="expandable-cell">' + data + '</div>';
                }
            },
            {
                "data": "id",
                "width": "10%",
                "render": function (data, type, row) {
                    debugger;
                    var html = `<div class="form-button-action text-center">`;

                    html += `<button class="btn btn-link text-primary btn-simple-primary btn-lg btn-edit" data-toggle="tooltip" title="Edit Folder View"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="modal" data-bs-target="#updateFolderModal"`;
                    html += `data-user-id="${data}" data-action="edit" onclick=editFolderViewBtn("/CompanyAdmin/FolderView/Update/${data}")>`;
                    html += `<i class="fa fa-edit"></i></button>`;

                    html += `<button class="btn btn-link text-danger btn-simple-danger btn-delete" data-toggle="tooltip" title="Delete File Type MetaData"`;
                    html += `data-original-title="Remove" data-bs-toggle="modal" data-bs-target="#deleteUserModal"`;
                    html += `data-action="delete" onclick=deleteFolderViewBtn("/CompanyAdmin/FolderView/Delete/${data}")>`;
                    html += `<i class="fa fa-trash"></i></button>`;

                    html += `</div > `;
                    return html;
                }
            }
        ]
    })
}

// Function for Delete Folder View
function deleteFolderViewBtn(url) {
    debugger;

    // Show a confirmation dialog using SweetAlert
    swal({
        title: 'Do you want delete this Folder View Document?',

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

// This function for Edit Folder View "GET" Method....
function editFolderViewBtn(url) {
    debugger;
    var modal = $('#updateFolderModal');
    rowCounter = 0;
    // Clear the metadata list before making the AJAX call
    $('#updateTableBody').empty();
    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            debugger;
            // Handle the response from the server
            if (response.success) {
                debugger;
                var folderView = response.data;
                $('#id').val(folderView.id);
                // Populate the filename input field
                $('#folderViewName_1').val(folderView.folderViewName);

                // Clear existing rows in the table
                $('#tableBody').empty();
                // Populate the metadata list
                var RowsData = folderView.rowsData;
                RowsData.forEach(function (data) {
                    debugger;
                    // Create a new row and add it to the table
                    UpdateEachRowData(true, data);
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
}

// This function for Update Folder View "POST" Method...
function updateBtn() {
    debugger;
    var form = $("#updateFolderViewForm");

    // Create an array to store row data
    var rowsData = [];

    for (var i = 1; i <= rowCounter; i++) {
        var rowId = "row_" + i; // Generate the row's ID
        var row = document.getElementById(rowId); // Get the row element

        // Check if the row exists before querying its properties
        if (row) {
            //var currentRowId = i;
            // Extract data from the row using class selectors or other methods
            //var idField = row.querySelector("#" + generateIdForField("Id", currentRowId));
            var id = row.querySelector(`#Id_${i}`).value;
            var selectObjectValue = row.querySelector("#selectObject_" + i).value;
            var objectTypeValue = row.querySelector("#objectType_" + i).value;
            var metaDataListId = "metaDataList_" + i;
            var metaDataListValue = row.querySelector("#" + metaDataListId).value;
            var operatorValue = row.querySelector("#operator_" + i).value;
            var entryValue = row.querySelector("[name='EntryValue']").value;
            var calendarValue = row.querySelector("[name='CalendarInput']").value;

            if (calendarValue) { entryValue = calendarValue }


            // Create an object to store row data
            var rowData = {
                Id: id,
                SelectObject: selectObjectValue,
                ObjectType: objectTypeValue,
                MetaDataList: metaDataListValue,
                Operator: operatorValue,
                Value: entryValue
            };

            // Add the row data to the array
            rowsData.push(rowData);
        }
    }

    // Create a FolderView object
    var folderViewData = {
        Id: $('#id').val(),
        FolderViewName: $("#folderViewName_1").val(), // You might need to adjust the selector
        RowsData: rowsData
    };

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: JSON.stringify(folderViewData),
        contentType: "application/json",
        success: function (response) {
            debugger;
            if (response.success) {
                debugger;

                // Hide the modal
                $("#updateFolderModal").modal("hide");
                not1(response.message);
                debugger;
                dataTable.ajax.reload();
            } else {
                if (response.isExistContainerNameError) {
                    // Display the error message on your view
                    $("#containerNameError").text(response.message);
                } else {
                    // Use SweetAlert for other error scenarios
                    swal({
                        title: 'Error',
                        text: 'An error occurred: ' + response.message,
                        icon: 'error'
                    });
                }
            }
        },
        error: function (error) {
            debugger;
            // Use SweetAlert for other error scenarios
            swal({
                title: 'Error',
                text: 'An error occurred: ' + error.message,
                icon: 'error'
            });
        }
    });
}

function UpdateEachRowData(isUpdateCase = false, data = null) {
    debugger;
    var tableBody = document.getElementById('updateTableBody');

    var newRow = document.createElement("tr");
    rowCounter++; // Increment rowCounter for the new row
    var currentRowId = rowCounter; // Capture the current row identifier
    newRow.id = "row_" + currentRowId;

    // Create and append table cells (td elements) to the row
    newRow.innerHTML = `
        <td class="text-center align-middle">
            <div class="select-with-arrow">
                <select for="SelectObject" name="SelectObject" class="form-control form-control-sm dropdown-toggle"
                        data-bs-toggle="dropdown" aria-required="true" id="selectObject_${rowCounter}">
                    <option disabled selected></option>  
                </select>
            </div>
        </td>
        <td class="text-center align-middle">
            <div class="select-with-arrow">
                <select for="ObjectType" name="ObjectType" class="form-control form-control-sm dropdown-toggle"
                        data-bs-toggle="dropdown" aria-required="true" id="objectType_${rowCounter}">
                    <option disabled selected></option>  
                </select>
            </div>
        </td>
        <td class="text-center align-middle">
            <div class="select-with-arrow">
                <select for="MetaDataList" name="MetaDataList" class="form-control form-control-sm dropdown-toggle"
                        data-bs-toggle="dropdown" aria-required="true" id="metaDataList_${rowCounter}">
                    <option disabled selected></option>                    
                </select>
            </div>
        </td>
        <td class="text-center align-middle">
            <div class="select-with-arrow">
                <select for="Operator" name="Operator" class="form-control form-control-sm dropdown-toggle"
                        data-bs-toggle="dropdown" aria-required="true" id="operator_${rowCounter}" onchange="toggleCalendar(${rowCounter})">
                    <option disabled selected></option>  
                </select>
            </div>
        </td>
        <td class="text-center align-middle">
                <!-- Add the calendar input with a unique ID -->
                <input type="date" name="CalendarInput" id="calendarInput_${rowCounter}" class="form-control form-control-sm d-none" />
                <!-- Add the text input with a unique ID -->
                <input type="text" name="EntryValue" id="entryValueInput_${rowCounter}" placeholder="ENTER VALUE (Text Field or Calendar Value)" class="form-control form-control-sm" />          
        </td>
        <td id="rowId_${rowCounter}" class="text-center align-middle" style="display: none;">
            <input type="hidden" id="Id_${rowCounter}" value="${isUpdateCase && data && data.id !== null ? data.id : '0'}">
        </td>
        <td class="text-center">
            <button type="button" class="btn btn-md btn-link text-success btn-add addRow" title="Add New Row" data-bs-placement="top">
                <i class="fa fa-plus-circle plus-icon"></i>
            </button>
            <button type="button" class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Delete Row" data-bs-placement="top">
                <i class="fa fa-trash"></i>
            </button>
        </td>
    `;

    // Append the new row to the table body
    tableBody.appendChild(newRow);

    var firstDropdown = newRow.querySelector(`#selectObject_${currentRowId}`);
    var secondDropdown = newRow.querySelector(`#objectType_${currentRowId}`);
    var metaDataDropdown = newRow.querySelector(`#metaDataList_${currentRowId}`);
    var operatorDropdown = newRow.querySelector(`#operator_${currentRowId}`);
    var entryValueField = newRow.querySelector("[name='EntryValue']");
    var calenderValueField = newRow.querySelector("[name='CalendarInput']");
    var idField = newRow.querySelector(`#Id_${currentRowId}`);

    var dateSelectedOperator = ["=", "!=", ">", ">=", "<", "<=", "is empty", "is not empty"];
    var selectedOperator = ["=", "!=", ">", ">=", "<", "<=", "Contains", "Does not Contain", "Starts With", "Does not Start with", "is empty", "is not empty"];

    // Add options to the first dropdown
    var firstDropdownOptions = [
        { text: "Document Type", value: "Document Type" },
        { text: "Container Type", value: "Container Type" }
    ];

    firstDropdownOptions.forEach(function (option) {
        var optionElement = document.createElement("option");
        optionElement.textContent = option.text;
        optionElement.value = option.value;
        firstDropdown.appendChild(optionElement);
    });

    // Clear the second and third dropdowns initially
    secondDropdown.innerHTML = "";
    metaDataDropdown.innerHTML = "";

    firstDropdown.addEventListener("change", function () {
        debugger;
        var selectedValue = firstDropdown.value;

        // Get the row identifier from the current dropdown's id
        var rowId = firstDropdown.id.split('_')[1];

        // Find the second and third dropdowns for the current row
        var currentSecondDropdown = document.getElementById(`objectType_${rowId}`);
        var currentMetaDataDropdown = document.getElementById(`metaDataList_${rowId}`);


        // Clear the second and third dropdowns
        secondDropdown.innerHTML = "";
        metaDataDropdown.innerHTML = "";

        // Check if options for the selected value have already been loaded
        if (!secondDropdownOptionsCache[selectedValue]) {
            // Make an AJAX request to the server to fetch the options
            fetch(`/CompanyAdmin/FolderView/GetSecondDropdownValue?selectedValue=${selectedValue}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Network response was not ok: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    debugger;
                    if (data.status) {
                        debugger;
                        secondDropdownOptionsCache[selectedValue] = data.data.map(item => item.containerTypeName || item.docTypeName);
                        // Populate the second dropdown based on the selected value
                        populateDropdown("DocumentType", selectedValue, rowId);

                    } else {
                        // Handle the case where the server returns an error or no data
                        console.error(data.message);
                    }
                })
                .catch(error => console.error(error));
        } else {
            // Populate the second dropdown based on the selected value (if options already loaded)
            populateDropdown("DocumentType", selectedValue, rowId);
        }
    });

    secondDropdown.addEventListener("change", function () {
        debugger;
        var selectedValue = firstDropdown.value;
        var selectedObject = secondDropdown.value;

        // Get the row identifier from the current dropdown's id
        var rowId = secondDropdown.id.split('_')[1];

        // Find the third dropdown for the current row
        var currentMetaDataDropdown = document.getElementById(`metaDataList_${rowId}`);

        // Clear the third dropdown
        currentMetaDataDropdown.innerHTML = "";

        // Check if options for the selected value have already been loaded
        if (!metaDataDropdownOptionsCache[selectedObject]) {
            // Make an AJAX request to the server to fetch the options
            fetch(`/CompanyAdmin/FolderView/GetMetaDataDropdownValue?selectedValue=${selectedValue}&selectedObject=${selectedObject}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Network response was not ok: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    debugger;
                    if (data.status) {
                        debugger;
                        if (selectedValue == "Document Type") {
                            metaDataDropdownOptionsCache[selectedObject] = data.data.map(item => item.metaDataName);
                        }
                        else {
                            metaDataDropdownOptionsCache[selectedObject] = data.data.map(item => item.metadataName);
                        }

                        // Populate the second dropdown based on the selected value
                        populateDropdown("MetaDataDropdown", selectedObject, rowId);
                    } else {
                        // Handle the case where the server returns an error or no data
                        console.error(data.message);
                    }
                })
                .catch(error => console.error(error));
        } else {
            // Populate the second dropdown based on the selected value (if options already loaded)
            populateDropdown("MetaDataDropdown", selectedObject, rowId);
        }
    })

    metaDataDropdown.addEventListener("change", function () {
        debugger;
        var selectedValue = metaDataDropdown.value;

        // Clear the operator dropdown initially
        operatorDropdown.innerHTML = "";

        // Check the selected metadata and populate the operator dropdown accordingly
        if (selectedValue === "Current Date") {
            // Populate with date operators
            dateSelectedOperator.forEach(function (operator) {
                var option = document.createElement("option");
                option.textContent = operator;
                option.value = operator;
                operatorDropdown.appendChild(option);
            });
        } else {
            // Populate with general operators
            selectedOperator.forEach(function (operator) {
                var option = document.createElement("option");
                option.textContent = operator;
                option.value = operator;
                operatorDropdown.appendChild(option);
            });
        }

        // Call toggleCalendar with the rowId to show/hide the appropriate input fields
        toggleCalendar(currentRowId);
    })


    // If it's an update case and data is provided, set the EntryValue
    if (isUpdateCase && data) {
        firstDropdown = newRow.querySelector(`#selectObject_${currentRowId}`);
        secondDropdown = newRow.querySelector(`#objectType_${currentRowId}`);
        metaDataDropdown = newRow.querySelector(`#metaDataList_${currentRowId}`);
        operatorDropdown = newRow.querySelector(`#operator_${currentRowId}`);
        entryValueField = newRow.querySelector("[name='EntryValue']");
        calenderValueField = newRow.querySelector("[name='CalendarInput']");

        // Set values for the dropdowns and input field based on 'data'
        firstDropdown.value = data.selectObject;
        // Simulate the change event to populate the second dropdown
        var firstDropdownEvent = new Event('change', { bubbles: true });
        firstDropdown.dispatchEvent(firstDropdownEvent);

        secondDropdown.value = data.objectType;
        var secondDropdownEvent = new CustomEvent('change', {
            bubbles: true,
            detail: {
                firstDropdownValue: firstDropdown.value,
                secondDropdownValue: secondDropdown.value
            }
        });
        secondDropdown.dispatchEvent(secondDropdownEvent);

        metaDataDropdown.value = data.metaDataList;
        var metaDataDropdownEvent = new CustomEvent('change', {
            bubbles: true,
            detail: {
                selectedValue: metaDataDropdown.value
            }
        });
        metaDataDropdown.dispatchEvent(metaDataDropdownEvent);
        operatorDropdown.value = data.operators;
        entryValueField.value = data.value;
        calenderValueField.value = data.value;

        idField.value = isUpdateCase && data.id !== null ? data.id : '0';
    }
}

function generateIdForField(fieldName, currentRowId) {
    return `${fieldName}_${currentRowId}`;
}

function toggleCalendar(rowId) {
    debugger;
    // Assuming you have unique IDs for each element based on rowCounter
    var operatorDropdown = document.getElementById(`metaDataList_${rowId}`);
    var entryValueInput = document.querySelector(`#entryValueInput_${rowId}`);
    var calendarInput = document.querySelector(`#calendarInput_${rowId}`);

    if (operatorDropdown.value === 'Current Date') {
        calendarInput.classList.remove('d-none');
        entryValueInput.classList.add('d-none');
    } else {
        calendarInput.classList.add('d-none');
        entryValueInput.classList.remove('d-none');
    }
}
