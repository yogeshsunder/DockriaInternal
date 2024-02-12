$(function () {
    $('#addAdminForm').submit(function (event) {
        event.preventDefault();

        if ($('#addAdminForm').valid()) {
            var data = {
                fullName: $('#fullName').val(),
                emailAddress: $('#emailAddress').val(),
                confirmEmailAddress: $('#confirmEmailAddress').val(),
                phoneNumber: $('#phoneNumber').val(),
            };

            $.ajax({
                type: 'POST',
                url: '/Admin/MasterAdmin/Create',
                data: JSON.stringify(data),
                contentType: 'application/json',

                success: function (result) {
                    if (result.success) {
                        // Show success message in the success modal
                        $('#successModalBody').html('Admin saved successfully!');
                        $('#successModal').modal('show');

                        $('#addAdminModal').modal('hide');
                        $('#admin-list').load('/Admin/MasterAdmin/AdminList');
                        $('#addAdminForm')[0].reset();
                        bindEditUser();
                    } else {
                        // Show specific error message from the server
                        if (result.errorMessage) {
                            $('#errorModalBody').html(result.errorMessage);
                        } else {
                            // Handle other validation errors here if needed
                            $('#errorModalBody').html('An error occurred while creating the Admin.');
                        }
                        $('#errorModal').modal('show');
                    }
                },
                error: function (xhr, status, error) {
                    // Show a generic error message to the user
                    console.error(xhr.responseText);
                    $('#errorModalBody').html('An error occurred while creating the Admin.');
                    $('#errorModal').modal('show');
                }
            });
        }
    });
});

// Initially hide all buttons except "CREATE NEW SMG"
$(document).ready(function () {
    $("#updateAdminForm, #deleteAdminForm").hide();
});
function updateAdminList(event) {
    if (event && event.preventDefault) {
        event.preventDefault();
    } else if (window.event) {
        window.event.returnValue = false;
    }

    if (!$("#addAdminForm").valid()) {
        return;
    }

    var data = {
        id: $('#id').val(),
        fullName: $('#fullName').val(),
        emailAddress: $('#emailAddress').val(),
        confirmEmailAddress: $('#confirmEmailAddress').val(),
        phoneNumber: $('#phoneNumber').val(),
    };

    $.ajax({
        type: 'POST',
        url: '/Admin/MasterAdmin/Edit',
        data: JSON.stringify(data),
        contentType: 'application/json',
        timeout: 10000,  // Set the timeout in milliseconds (e.g., 10 seconds)

        success: function (result) {
            if (result.success) {
                // Show success message in the success modal
                $('#successModalBody').html('Admin Data updated successfully!');
                $('#successModal').modal('show');

                // Delay page reload for 2 seconds (adjust as needed)
                setTimeout(function () {
                    // Reset forms and reload the page
                    $('#addAdminForm')[0].reset();
                    location.reload();
                }, 2000); // 2000 milliseconds = 2 seconds
            } else {
                // Show specific error message from the server
                if (result.errorMessage) {
                    $('#errorModalBody').html(result.errorMessage);
                } else {
                    // Handle other validation errors here if needed
                    $('#errorModalBody').html('An error occurred while updating the Admin.');
                }
                $('#errorModal').modal('show');
            }
        },
        error: function (xhr, status, error) {
            // Show a generic error message to the user
            console.error(xhr.responseText);
            $('#errorModalBody').html('An error occurred while updating the Admin.');
            $('#errorModal').modal('show');
        }
    });
}





function bindEditUser() {
    $('.edit-user').on('click', function () {
        var itemid = $(this).data('id');

        $.ajax({
            url: '/Admin/MasterAdmin/Edit/' + itemid,
            type: 'GET',
            dataType: 'html',
            success: function (response) {
                $('#addAdminModal').html(response);
                // Show the update button and hide the save and delete buttons
                $('#updateAdminForm').show();
                $('#saveAdminForm, #deleteAdminForm').hide();
                // Assuming you have a button with id 'updateButton', bind the click event
                $('#updateButton').on('click', updateAdminList);
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    });
}

$(document).ready(function () {
    bindEditUser();
});




function deleteAdminList(formId) {
    debugger
    var adminId = $('#id').val();
    var confirmDelete = confirm('Do you want to delete this admin?');

    if (confirmDelete) {
        $.ajax({
            url: '/Admin/MasterAdmin/Delete/' + adminId,
            type: 'POST',
            dataType: 'json',
            success: function (result) {
                if (result.success) {
                    alert('Master Admin Deleted successfully');
                    if (formId === '#addAdminForm') {
                        $('#' + formId)[0].reset();
                    }
                    $('#admin-list').load('/Admin/MasterAdmin/AdminList', function () {
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    });
                } else {
                    alert('Please click Edit Button on Company Admin list first, then click Delete Button....');
                }
            },
            error: function (xhr, status, error) {
                alert('An error occurred while deleting the admin.');
            }
        });
    }
}



function deleteBtn(formId) {
    debugger;

    // Extract id from the formId object
    var id = formId;

    // Make an AJAX call to get user details
    $.ajax({
        url: '/Admin/MasterAdmin/GetDetails/' + id,
        type: 'GET',
        dataType: 'html',
        success: function (userData) {
            debugger;

            // Display user details
            $('#addAdminModal').html(userData);

            // Set up a custom confirmation message
            var customConfirmMsg = 'Do you want to delete this Company?';

            // Display a Bootstrap modal for additional confirmation
            $('#confirmationModal .modal-body').html(customConfirmMsg);
            $('#confirmationModal').modal('show');
        },
        error: function (xhr, status, error) {
            // Handle AJAX error during user details retrieval
            console.error('Details Error:', status, error);
        }
    });

    // Set up event handler for confirmation button using event delegation
    $(document).off('click', '#confirmBtn').on('click', '#confirmBtn', function () {
        debugger;

        // Close the Bootstrap modal
        $('#confirmationModal').modal('hide');

        // Proceed with the deletion
        performDeletion(id, formId);
    });
}

function performDeletion(id, formId) {
    // Make an AJAX call to delete the Master Admin
    $.ajax({
        url: '/Admin/MasterAdmin/Delete/' + id,
        type: 'POST',
        dataType: 'json',
        success: function (result) {
            if (result && result.success) {
                // Show success message in the success modal
                $('#successModalBody').html('Master Admin Deleted successfully');
                $('#successModal').modal('show');

                if (formId === '#addAdminForm') {
                    $('#' + formId)[0].reset();
                }

                // Update the admin list if needed
                updateAdminList();
                location.reload();

            } else {
                // Show error message in the success modal (to make it look similar)
                $('#successModalBody').html(result.message || 'Master Admin Deleted successfully.');
                $('#successModal').modal('show');
            }
        },
        error: function (xhr, status, error) {
            // Show error message in the success modal (to make it look similar)
            $('#successModalBody').html('Master Admin Deleted successfully');
            $('#successModal').modal('show');
        }
    });
}
// Add this code where you handle your modals
$(document).on('click', '#okButton', function () {
    location.reload(); // Reload the entire page
});
