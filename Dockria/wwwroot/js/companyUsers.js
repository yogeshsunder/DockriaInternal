var dataTable = null;

$(document).ready(function () {
    loadDataTable();

    $('.dataTables_wrapper input[type="search"]').on('focus', function () {
        debugger;
        // Add a class to hide the icon when input is focused
        $('.dataTables_wrapper').addClass('search-focused');
    });

    $('.dataTables_wrapper input[type="search"]').on('blur', function () {
        debugger;
        // Remove the class to show the icon when input loses focus
        $('.dataTables_wrapper').removeClass('search-focused');
    });
});


function loadDataTable() {
    debugger;
    dataTable = $('#responsive-datatable').DataTable({

        "ajax": {
            "url": "/CompanyAdmin/CompanyAdmin/GetAll"
        },
        "ordering": true,
        "responsive": true,


        "columns": [
            {
                "data": "fullName",
                "width": "17%",
                "render": function (data) {
                    return '<div class="text-truncate">' + data + '</div>';
                }
            },
            {
                "data": "email",
                "width": "17%",
                "render": function (data) {
                    return '<div class="text-truncate">' + data + '</div>';
                }
            },
            {
                "data": "department",
                "width": "17%",
                "render": function (data) {
                    return '<div class="text-truncate">' + data + '</div>';
                }
            },
            {
                "data": "mobileNumber",
                "width": "17%",
                "render": function (data) {
                    return '<div class="text-truncate">' + data + '</div>';
                }
            },


            {
                "data": null,
                "width": "17%",
                "render": function (data) {
                    var permissions = "";

                    if (data.readPermission) {
                        permissions += `<input type="checkbox" disabled checked /> Read `;
                    } else {
                        permissions += `<input type="checkbox" disabled /> Read `;
                    }

                    if (data.writePermission) {
                        permissions += `<br/><input type="checkbox" disabled checked /> Write `;
                    } else {
                        permissions += `<br/><input type="checkbox" disabled /> Write `;
                    }

                    if (data.uploadPermission) {
                        permissions += `<br/><input type="checkbox" disabled checked /> Upload `;
                    } else {
                        permissions += `<br/><input type="checkbox" disabled /> Upload `;
                    }

                    return `<div class="permissions-group">${permissions}</div>`;
                }
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    debugger;
                    var html = `<div class="form-button-action offset-1">`;

                    html += `<button class="btn btn-link text-primary btn-simple-primary btn-lg btn-edit" title="Edit User"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="tooltip" data-bs-target="#editUserModal"`;
                    html += `data-user-id="${data}" data-action="edit" onclick="editUser(this)">`;
                    html += `<i class="fa fa-edit"></i></button>`;

                    //html += `<button class="btn btn-link text-danger btn-simple-danger btn-delete" title="Delete User"`;
                    //html += `data-original-title="Remove" data-bs-toggle="tooltip" data-bs-target="#deleteUserModal"`;
                    //html += `data-user-id="${data.id}" data-action="delete" onclick="confirmDeleteUser('/CompanyAdmin/CompanyAdmin/DeleteUser/${data.id}', ${JSON.stringify(data)})">`;
                    //html += `<i class="fa fa-trash"></i></button>`;

                    html += `<button class="btn btn-link text-danger btn-simple-danger btn-delete" title="Delete User"`;
                    html += `data-original-title="Remove" data-bs-toggle="tooltip" data-bs-target="#deleteUserModal"`;
                    html += `data-user-id="${data}" data-action="delete" onclick="confirmDeleteUser(this)">`;
                    html += `<i class="fa fa-trash"></i></button>`;


                    if (row.applicationUser != null) {
                        if (row.applicationUser.lockoutEnd == null || new Date(row.applicationUser.lockoutEnd) < new Date()) {
                            html += '<a id="' + data + '" onclick="LockUnlock(\'' + data + '\')" class="btn text-primary mx-2 btn-sm btn-lock"';
                            html += 'data-bs-toggle="tooltip" title="Lock" data-bs-placement="bottom" data-bs-original-title="lock">';
                            html += '<span><i class="fa fa-lock"></i></span></a>';
                        } else if (new Date(row.applicationUser.lockoutEnd) > new Date()) {
                            html += '<a id="' + data + '" onclick="LockUnlock(\'' + data + '\')" class="btn text-danger mx-2 btn-sm btn-unlock"';
                            html += 'data-bs-toggle="tooltip" title="Unlock" data-bs-placement="bottom" data-bs-original-title="Unlock">';
                            html += '<span><i class="fa fa-unlock"></i></span></a>';
                        }
                    } else {
                        html += '<a id="' + data + '" onclick="LockUnlock(\'' + data + '\')" class="btn text-primary mx-2 btn-sm btn-lock"';
                        html += 'data-bs-toggle="tooltip" title="Lock" data-bs-placement="bottom" data-bs-original-title="lock">';
                        html += '<span><i class="fa fa-lock"></i></span></a>';
                    }

                    html += `</div>`;
                    return html;
                }
            }
        ]
    })
}


// Function for CompanyUsers Lock & Unlock line 58 to 89
function LockUnlock(id) {
    debugger;

    // Get the button element by its ID
    //var lockButton = document.getElementById(id);
    $.ajax({
        type: "POST",
        url: "/CompanyAdmin/CompanyAdmin/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            debugger;
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    })
}

// Function Update CompanyUser Data
$(document).ready(function () {
    var updateUserForm = $('#updateUserForm');
    var editUserModal = new bootstrap.Modal(document.getElementById('editUserModal'));
    var errorModal = new bootstrap.Modal(document.getElementById('errorModal'));

    // Show/hide the error messages based on field values and focus
    $('#email').on('input', function () {
        var emailInput = this;
        if (emailInput.value.trim() === '') {
            $('#emailError').text('* Please Enter The Email.');
        } else if (!isValidEmail(emailInput.value)) {
            $('#emailError').text('* Please Enter a Valid Email.');
        } else {
            $('#emailError').text('');
        }
    });

    $('#firstName').on('input', function () {
        var firstName = this.value.trim();
        if (firstName === '') {
            $('#firstNameError').text('* Please Enter The First Name.');
        } else {
            $('#firstNameError').text('');
        }
    });

    $('#lastName').on('input', function () {
        var lastName = this.value.trim();
        if (lastName === '') {
            $('#lastNameError').text('* Please Enter The Last Name.');
        } else {
            $('#lastNameError').text('');
        }
    });

    $('#department').on('input', function () {
        var department = this.value.trim();
        if (department === '') {
            $('#departmentError').text('* Please Enter the Department.');
        } else {
            $('#departmentError').text('');
        }
    });

    $('#phoneNumber').on('input', function () {
        var phoneNumber = this.value.trim();
        if (phoneNumber === '') {
            $('#phoneError').text('* Please enter the PhoneNumber.');
        } else {
            $('#phoneError').text('');
        }
    });

    // Handle input events for all three checkboxes
    $('#readPermission, #writePermission, #uploadPermission').on('input', function () {
        var readPermission = $('#readPermission').is(':checked');
        var writePermission = $('#writePermission').is(':checked');
        var uploadPermission = $('#uploadPermission').is(':checked');

        if (!readPermission && !writePermission && !uploadPermission) {
            $('#permissionsError').text('* Please Select At Least One Permission.');
        } else {
            $('#permissionsError').text('');
        }
    });

    // Handle form submission
    $('#updateUserBtn').click(function (event) {
        debugger;
        event.preventDefault();

        var emailInput = document.getElementById("email");
        var firstName = $('#firstName').val();
        var lastName = $('#lastName').val();
        var department = $('#department').val();
        var phoneNumber = $('#phoneNumber').val();
        var readPermission = $('#readPermission').is(':checked');
        var writePermission = $('#writePermission').is(':checked');
        var uploadPermission = $('#uploadPermission').is(':checked');

        // Create an object to store the form data
        var user = {
            Id: $('#userId').val(),
            Email: $('#email').val(),
            FirstName: $('#firstName').val(),
            LastName: $('#lastName').val(),
            Department: $('#department').val(),
            MobileNumber: $('#phoneNumber').val(),
            ReadPermission: $('#readPermission').is(':checked'),
            WritePermission: $('#writePermission').is(':checked'),
            UploadPermission: $('#uploadPermission').is(':checked')
        };

        $('.error-message').text(''); // Clear any previous error messages

        // Simple validation to check if required fields are filled
        var isValid = true;
        if (emailInput.value.trim() === '') {
            $('#emailError').text('* Please Enter The Email.');
            isValid = false;
        } else if (!isValidEmail(emailInput.value)) {
            $('#emailError').text('* Please Enter a Valid Email.');
            isValid = false;
        }

        if (firstName.trim() === '') {
            $('#firstNameError').text('* Please Enter The First Name.');
            isValid = false;
        }

        if (lastName.trim() === '') {
            $('#lastNameError').text('* Please Enter The Last Name.');
            isValid = false;
        }

        if (department.trim() === '') {
            $('#departmentError').text('* Please Enter the Department.');
            isValid = false;
        }


        $(document).ready(function () {
            debugger;
            $('#phoneNumber').on('input', function () {
                var phoneNumber = this.value.trim();
                var phoneError = $('#phoneError');

                // Check if the phone number is empty
                if (phoneNumber === '') {
                    phoneError.text('* Please enter the phone number.');
                } else {
                    // Check the format using the regular expression
                    var pattern = /^\d{10}$/; // Assumes a 10-digit phone number without hyphens

                    // Validate the phone number against the pattern
                    if (!pattern.test(phoneNumber)) {
                        phoneError.text('* Invalid phone number format. Please use a 10-digit format.');
                    } else {
                        phoneError.text(''); // Clear the error message when the format is correct
                    }
                }
            });
        });

        // Check if at least one permission is selected
        if (!readPermission && !writePermission && !uploadPermission) {
            $('#permissionsError').text('* Please Select At Least One Permission.');
            isValid = false;
        }

        if (!isValid) {
            return;
        }


        // Send the AJAX request to save the user
        $.ajax({
            url: updateUserForm.attr('action'),
            type: updateUserForm.attr('method'),
            data: user,
            success: function (response) {
                if (response) {
                    resetEditUserModal();
                    // Hide the modal
                    $('#editUserModal').modal('hide');
                    toastr.success(response.message);

                    // Reload the page after a delay of 2 seconds
                    setTimeout(function () {
                        dataTable.ajax.reload();
                    }, 2000);
                } else {
                    // Handle error response
                    // ...
                }
            },
            error: function (error) {
                // Handle AJAX error
                // ...
            }
        });
    });
});


function resetEditUserModal() {

    debugger;
    $('#email').val(''); // Reset the text input field
    $('#firstName').val('');
    $('#lastName').val('');
    $('#department').val('');
    $('#phoneNumber').val('');
    // Uncheck the OCR Checkbox
    $('#readPermission').prop('checked', false);
    $('#writePermission').prop('checked', false);
    $('#uploadPermission').prop('checked', false);// Uncheck the OCR Checkbox
    // Clear the metadata table

    // Reset the firstLoad variable to true
    firstLoad = true;

    //  setInitialValues();
}
function showError(elementId, message) {
    $('#' + elementId).text(message);
}

// Create CompanyUser Data Function
$(document).ready(function () {
    debugger;
    var addUserForm = $('#addUserForm');
    var addUserModal = new bootstrap.Modal(document.getElementById('addUserModal'));
    var errorModal = new bootstrap.Modal(document.getElementById('errorModal'));

    // Show/hide the error messages based on field values and focus
    $('#email1').on('input', function () {
        var emailInput = this;
        if (emailInput.value.trim() === '') {
            $('#emailError1').text('* Please Enter The Email.');
        } else if (!isValidEmail(emailInput.value)) {
            $('#emailError1').text('* Please Enter a Valid Email.');
        } else {
            $('#emailError1').text('');
        }
    });

    $('#firstName1').on('input', function () {
        var firstName = this.value.trim();
        if (firstName === '') {
            $('#firstNameError1').text('* Please Enter The First Name.');
        } else {
            $('#firstNameError1').text('');
        }
    });

    $('#lastName1').on('input', function () {
        var lastName = this.value.trim();
        if (lastName === '') {
            $('#lastNameError1').text('* Please Enter The Last Name.');
        } else {
            $('#lastNameError1').text('');
        }
    });

    $('#department1').on('input', function () {
        var department = this.value.trim();
        if (department === '') {
            $('#departmentError1').text('* Please Enter the Department.');
        } else {
            $('#departmentError1').text('');
        }
    });

    $('#phoneNumber1').on('input', function () {
        debugger;
        var phoneNumber = this.value.trim();
        var phoneError = $('#phoneError1');

        if (phoneNumber === '') {
            phoneError.text('* Please enter the PhoneNumber.');
        } else {
            // Check the format using the regular expression
            var pattern = /\d{3}[\-]\d{3}[\-]\d{4}/;
            if (pattern.test(phoneNumber)) {
                phoneError.text('* Invalid phone number format. Please use the format: XXX-XXX-XXXX');
            } else {
                phoneError.text(''); // Clear the error message when the format is correct
            }
        }
    });



    // Handle input events for all three checkboxes
    $('#readPermission1, #writePermission1, #uploadPermission1').on('input', function () {
        var readPermission = $('#readPermission1').is(':checked');
        var writePermission = $('#writePermission1').is(':checked');
        var uploadPermission = $('#uploadPermission1').is(':checked');

        if (!readPermission && !writePermission && !uploadPermission) {
            $('#permissionsError1').text('* Please Select At Least One Permission.');
        } else {
            $('#permissionsError1').text('');
        }
    });

    // Handle form submission
    $('#addUserSubmitButton').click(function (event) {
        debugger;
        event.preventDefault();

        var emailInput = document.getElementById("email1");
        var firstName = $('#firstName1').val();
        var lastName = $('#lastName1').val();
        var department = $('#department1').val();
        var phoneNumber = $('#phoneNumber1').val();
        var readPermission = $('#readPermission1').is(':checked');
        var writePermission = $('#writePermission1').is(':checked');
        var uploadPermission = $('#uploadPermission1').is(':checked');

        $('.error-message').text(''); // Clear any previous error messages

        // Simple validation to check if required fields are filled
        var isValid = true;
        if (emailInput.value.trim() === '') {
            $('#emailError1').text('* Please Enter The Email.');
            isValid = false;
        } else if (!isValidEmail(emailInput.value)) {
            $('#emailError1').text('* Please Enter a Valid Email.');
            isValid = false;
        }

        if (firstName.trim() === '') {
            $('#firstNameError1').text('* Please Enter The First Name.');
            isValid = false;
        }

        if (lastName.trim() === '') {
            $('#lastNameError1').text('* Please Enter The Last Name.');
            isValid = false;
        }

        if (department.trim() === '') {
            $('#departmentError1').text('* Please Enter the Department.');
            isValid = false;
        }

        if (phoneNumber.trim() === '') {
            $('#phoneError1').text('* Please enter the PhoneNumber.');
            isValid = false;
        }

        // Check if at least one permission is selected
        if (!readPermission && !writePermission && !uploadPermission) {
            $('#permissionsError1').text('* Please Select At Least One Permission.');
            isValid = false;
        }

        if (!isValid) {
            return;
        }

        // Create an object with additional user details if needed
        var user = {
            Email: $('#email1').val(),
            FirstName: $('#firstName1').val(),
            LastName: $('#lastName1').val(),
            Department: $('#department1').val(),
            MobileNumber: $('#phoneNumber1').val(),
            ReadPermission: $('#readPermission1').is(':checked'),
            WritePermission: $('#writePermission1').is(':checked'),
            UploadPermission: $('#uploadPermission1').is(':checked')
        };

        // Send the AJAX request to save the user
        // Send the AJAX request to save the user
        $.ajax({
            url: addUserForm.attr('action') + '?_=' + new Date().getTime(), // Add cache buster
            type: addUserForm.attr('method'),
            data: JSON.stringify(user),
            contentType: 'application/json',
            success: function (response) {
                debugger;
                console.log('AJAX Success:', response);

                // Clear the DataTable and reload data
                // dataTable.clear().draw();
                if (response) {
                    resetAddUserModal();
                    // Hide the modal
                    $('#addUserModal').modal('hide');
                    toastr.success(response.message);

                    // Reload the page after a delay of 2 seconds
                    setTimeout(function () {
                        dataTable.ajax.reload();
                        console.log('DataTable reloaded');
                    });
                } else {
                    // Handle error response
                    //console.error('Error in response:', response);
                    // ...
                }
            },
            error: function (error) {
                // Handle AJAX error
                //console.error('AJAX Error:', error);
                // ...
            }
        });

    });
});




function resetAddUserModal() {
    debugger;
    $('#email1').val(''); // Reset the text input field
    $('#firstName1').val('');
    $('#lastName1').val('');
    $('#department1').val('');
    $('#phoneNumber1').val('');
    // Uncheck the OCR Checkbox
    $('#readPermission1').prop('checked', false);
    $('#writePermission1').prop('checked', false);
    $('#uploadPermission1').prop('checked', false);// Uncheck the OCR Checkbox
    // Clear the metadata table

    // Reset the firstLoad variable to true
    firstLoad = true;

    //  setInitialValues();
}


function showError(elementId, message) {
    $('#' + elementId).text(message);
}
function isValidEmail(email) {
    // Regular expression for email validation
    var emailPattern = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;
    return emailPattern.test(email);
}

// Function to show error message using SweetAlert Line 309 to 320
function showErrorMessage(message) {
    debugger;
    var errorMessages = message.split("<br>");
    var formattedErrorMessage = errorMessages.join("\n");
    swal({
        icon: 'error',
        title: 'Error',
        text: formattedErrorMessage,
        confirmButtonColor: '#3085d6',
        confirmButtonText: 'OK'
    });
}

// Function to handle the show Company User data when Edit click event
function editUser(button) {
    debugger;
    // Get the user ID from the data attribute of the button
    var userId = $(button).data('user-id');

    // Set the user ID in the modal form
    $('#editUserModal').find('#userId').val(userId);

    // Clear the error messages
    $('.error-message').text('');

    // Open the edit user modal
    $('#editUserModal').modal('show');

    // Send an AJAX request to fetch the CompanyUser data
    $.ajax({
        url: '/CompanyAdmin/CompanyAdmin/GetUser', // Replace 'ControllerName' with the actual name of the controller containing the GetUser action
        type: 'GET',
        data: { id: userId },
        success: function (response) {
            // Clear the form fields
            $('#updateUserForm')[0].reset();

            // Populate the form fields with the retrieved data
            $('#email').val(response.email);
            $('#firstName').val(response.firstName);
            $('#lastName').val(response.lastName);
            $('#department').val(response.department);
            $('#phoneNumber').val(response.mobileNumber);
            $('#readPermission').prop('checked', response.readPermission);
            $('#writePermission').prop('checked', response.writePermission);
            $('#uploadPermission').prop('checked', response.uploadPermission);
        },
        error: function (error) {
            // Handle the error
        }
    });
}

function confirmDeleteUser(button) {
    debugger;
    // Get the user ID from the data attribute of the button
    var userId = $(button).data('user-id');

    // Set the user ID in the modal form
    $('#deleteUserModal').find('#userId').val(userId);

    // Clear the error messages
    $('.error-message').text('');

    // Open the edit user modal
    $('#deleteUserModal').modal('show');

    // Send an AJAX request to fetch the CompanyUser data
    $.ajax({
        url: '/CompanyAdmin/CompanyAdmin/GetUser', // Replace 'ControllerName' with the actual name of the controller containing the GetUser action
        type: 'GET',
        data: { id: userId },
        success: function (userData) {
            debugger;
            // Show a confirmation dialog using SweetAlert
            if (userData) {
                swal({
                    title: `Do you want to delete user ${userData.firstName} ${userData.lastName}?`,
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
                        // Perform the AJAX request to delete the user
                        $.ajax({
                            url: '/CompanyAdmin/CompanyAdmin/DeleteUser/' + userId, // Replace with the correct URL
                            type: 'DELETE',
                            success: function (response) {
                                debugger;
                                // Handle the response from the server
                                if (response && response.success) {
                                    toastr.success(response.message);
                                    // Reload the page after a delay of 2 seconds
                                    dataTable.ajax.reload();
                                } else {
                                    // Handle error response
                                    toastr.error(response.message);
                                }
                            },
                            error: function (error) {
                                // Handle AJAX error
                                toastr.error('An error occurred during the request.');
                            }
                        });
                    }
                });
            } else {
                // Handle the case where user data is not available
                toastr.error('User data not found.');
            }
        },
        error: function (error) {
            // Handle AJAX error when fetching user details
            toastr.error('An error occurred while fetching user details.');
        }
    });
}





/*function isNumberKey(event, numDigits) {
    debugger;
    var charCode = (event.which) ? event.which : event.keyCode;
    var phoneNumber = event.target.value;

    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        alert("Please input only digit number....");
        return false;
    }

    // Restrict the length of the phone number
    if (phoneNumber.length >= numDigits) {
        return false;
    }

    return true;
}*/

// This fun// Assuming you're using jQuery
$(document).ready(function () {
    debugger;
    $('#submitButton').on('click', function () {
        submitForm();
    });
});


function submitForm() {
    debugger;
    var fileInput = document.getElementById("userDataFile");
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append("formFile", file);

    $.ajax({
        url: "/CompanyAdmin/CompanyAdmin/BulkUserAddition",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            debugger;
            if (response.success) {
                debugger;
                var successMessages = response.successMessage || [];
                var errorMessages = response.errorMessage || [];

                if (successMessages.length > 0 || errorMessages.length > 0) {
                    var errorMessageElement = document.getElementById('bulkErrorMessage');
                    errorMessageElement.innerHTML = ''; // Clear previous messages

                    if (successMessages.length > 0) {
                        var successHeading = document.createElement('h4');
                        successHeading.textContent = 'Success Messages';
                        errorMessageElement.appendChild(successHeading);

                        var successMessageList = document.createElement('ul');
                        successMessageList.classList.add('success-message-list');

                        for (var i = 0; i < successMessages.length; i++) {
                            var messageItem = document.createElement('li');
                            messageItem.textContent = successMessages[i];
                            successMessageList.appendChild(messageItem);
                        }

                        errorMessageElement.appendChild(successMessageList);
                    }

                    if (errorMessages.length > 0) {
                        var errorHeading = document.createElement('h4');
                        errorHeading.textContent = 'Error Messages';
                        errorMessageElement.appendChild(errorHeading);

                        var errorMessageList = document.createElement('ul');
                        errorMessageList.classList.add('error-message-list');

                        for (var i = 0; i < errorMessages.length; i++) {
                            var messageItem = document.createElement('li');
                            messageItem.textContent = errorMessages[i];
                            errorMessageList.appendChild(messageItem);
                        }

                        errorMessageElement.appendChild(errorMessageList);
                    }

                    // Hide the "Bulk User Addition" modal
                    $('#bulkUserModal').modal('hide');

                    var errorModal = new bootstrap.Modal(document.getElementById('bulkModal'));
                    errorModal.show();
                }
                // Reload the CompanyAdmin/CompanyAdmin page after a delay of ten seconds
                setTimeout(function () {
                    window.location.href = '/CompanyAdmin/CompanyAdmin';
                }, 5000);
            } else {
                debugger;
                var errorMessage = "";

                if (response.errorMessage && response.errorMessage.length > 0) {
                    var errorMessageElement = document.getElementById('bulkErrorMessage');
                    errorMessageElement.innerHTML = ''; // Clear previous messages

                    var errorMessageHeading = document.createElement('h4');
                    errorMessageHeading.textContent = 'Error Messages';
                    errorMessageHeading.classList.add('error-message-list');
                    errorMessageElement.appendChild(errorMessageHeading);

                    var errorMessageList = document.createElement('ul');

                    for (var i = 0; i < response.errorMessage.length; i++) {
                        var messageItem = document.createElement('li');
                        messageItem.textContent = response.errorMessage[i];
                        errorMessageList.appendChild(messageItem);
                    }

                    errorMessageElement.appendChild(errorMessageList); // Add the unordered list to the error message element
                } else {
                    errorMessage = 'Failed to create user. Please try again.';
                }

                // Hide the "Bulk User Addition" modal
                $('#bulkUserModal').modal('hide');

                var errorModal = new bootstrap.Modal(document.getElementById('bulkModal'));
                errorModal.show();

                // Reload the CompanyAdmin/CompanyAdmin page after a delay of ten seconds
                setTimeout(function () {
                    window.location.href = '/CompanyAdmin/CompanyAdmin';
                }, 5000);
            }
        },
        error: function (error) {
            if (error.responseJSON && error.responseJSON.errors && error.responseJSON.errors.length > 0) {
                var errorMessage = error.responseJSON.errors.join('<br>');
                showErrorMessage(errorMessage);
            } else {
                showErrorMessage('An error occurred while creating the user. Please try again later.');
            }
        }
    });
}

// Download Excel File
function exportExcel() {
    window.location.href = '/CompanyAdmin/CompanyAdmin/ExportExcel';
}

// Download PDF File
function exportPdf() {
    window.location.href = '/CompanyAdmin/CompanyAdmin/ExportPdf';
}