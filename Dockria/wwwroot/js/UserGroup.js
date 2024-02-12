var dataTable;
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
    dataTable = $('#addRowData').DataTable({
        "ajax": {
            "url": "/CompanyAdmin/UserGroup/GetAll",
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "groupName", "width": "65%" },
            {
                "data": "id",
                "width": "35%",
                "render": function (data, type, full, meta) {
                    var html = `
        <div class="form-button-action text-center hover-container">
            <button class="btn btn-link text-primary btn-simple-primary btn-lg btn-edit" 
                data-toggle="tooltip" title="Edit" 
                data-original-title="Edit Task" 
                data-bs-toggle="modal" 
                data-bs-target="#editUserModal" 
                data-user-id="${data}" 
                data-action="edit" 
                onclick="editGroupUser(this)">
                <i class="fa fa-edit"></i>
            </button>
            <button class="btn btn-link text-danger btn-simple-danger btn-delete" 
                data-toggle="tooltip" title="Delete" 
                data-original-title="Remove" 
                data-bs-toggle="modal" 
                data-action="delete" 
                data-group-name="${full.groupName}" 
                onclick="DeleteGroup('/CompanyAdmin/UserGroup/DeleteGroup/${data}','${full.groupName}')">
                <i class="fa fa-trash"></i>
            </button>
        </div>`;
                    return html;
                }
            }
        ]
    });
}

function DeleteGroup(url, groupName) {
    swal({
        title: `Do you want to delete the group "${groupName}"?`,
        icon: "warning",
        buttons: {
            delete: {
                text: 'Delete',
                value: true,
                dangerMode: true,
            },
            cancel: 'Cancel',
        },
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (error) {
                    toastr.error("An error occurred while deleting the data.");
                }
            });
        }
    });
}


$(document).ready(function () {
    showUserList();

    $(document).ready(function () {
        debugger;
        $('#userSearchInput').on('input', function () {
            debugger;
            var searchTerm = $(this).val().toLowerCase();
            filterOptions(searchTerm);
        });

      
    });


    $('#userSearch').on('input', function () {
        debugger;
        var searchTerm = $(this).val().toLowerCase();
        filterOptions1(searchTerm);
    });


    function filterOptions1(searchTerm) {
        var options = $('.user-list1 .form-check');
        options.each(function () {
            var optionText = $(this).find('.form-check-label').text().toLowerCase();
            var optionMatchesSearch = optionText.includes(searchTerm);

            $(this).toggle(optionMatchesSearch);
        });
    }

    function showUserList() {
        $.ajax({
            url: '/CompanyAdmin/UserGroup/UserName',
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                populateCheckboxList('.user-list', data.userlist);
            },
            error: function (error) {
                console.error(error.statusText);
            }
        });
    }
    function populateCheckboxList(selector, options) {
        var checkboxList = $(selector);
        checkboxList.empty();
        options.sort(function (a, b) {
            return a.text.localeCompare(b.text);
        });

        $.each(options, function (index, option) {
            var checkbox = $('<div class="form-check checkbox-item"></div>');
            checkbox.append($('<input>', {
                type: 'checkbox',
                id: 'user_' + option.value,
                value: option.value,
                class: 'form-check-input',
                name: 'userList'
            }));
            checkbox.append($('<label>', {
                for: 'user_' + option.value,
                class: 'form-check-label',
                text: option.text,
                style: 'font-size: 18px;'
            }));

            checkboxList.append(checkbox);
        });
    }
    function filterOptions(searchTerm) {
        var options = $('.user-list .form-check');
        options.each(function () {
            var optionText = $(this).find('.form-check-label').text().toLowerCase();
            var optionMatchesSearch = optionText.includes(searchTerm);

            $(this).toggle(optionMatchesSearch);
        });


       
    }
});



// This is for Add New Group "POST" Method....
$(document).ready(function () {
    $('#addUserSubmitButton').click(function (e) {
        e.preventDefault();

        var addUserForm = $('#addUserForm');
        var addUserModal = $('#addUserModal');
        var selectedUsers = $('input[name="userList"]:checked').map(function () {
            return $(this).siblings('.form-check-label').text();
        }).get();

        $('.error-message').text('');

        var userGroupNameInput = $('#GroupName');
        var userGroupName = userGroupNameInput.val().trim();
        var groupnameError = '';
        if (userGroupName === '') {
            groupnameError = '* Please enter Group name';
            userGroupNameInput.on('input', function () {
                if (userGroupNameInput.val().trim() !== '') {
                    $('#groupError1').text('');
                }
            });
        }

        var usernameError = '';
        if (selectedUsers.length === 0) {
            usernameError = '* Please select at least one user';
        }

        $('#groupError1').text(groupnameError);
        $('#userGroupError1').text(usernameError);

        if (groupnameError || usernameError) {
            return;
        }

        var user = {
            UserGroupName: userGroupName,
            UserName: selectedUsers.join(", ")
        };

        $.ajax({
            url: addUserForm.attr('action'),
            type: addUserForm.attr('method'),
            data: JSON.stringify(user),
            contentType: 'application/json',
            success: function (response) {
                if (response) {
                    // Hide the modal
                    resetUserModal();
                    $('#addUserModal').modal('hide');
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

    function resetUserModal() {

        debugger;
        $('#GroupName').val(''); // Reset the text input field
        $('#groupError1').val('');
     
        // Clear the metadata table

        $('input[name="userList"]').prop('checked', false);
        // Reset the firstLoad variable to true
        firstLoad = true;

        //  setInitialValues();
    }
function showError(elementId, message) {
    $('#' + elementId).text(message);
}

    $('#GroupName').on('focus', function () {
        hideErrorMessage();
    });

    $('#addUserForm').on('change', 'input[name="userList"]', function () {
        var usernameError = '';
        if ($('input[name="userList"]:checked').length === 0) {
            usernameError = '* Please select at least one user';
        }

        $('#userGroupError1').text(usernameError);
    });
});

function showErrorMessage(message) {
    var errorMessages = message.split("<br>");
    var formattedErrorMessage = errorMessages.join("\n");
    swal({
        icon: 'error',
        title: 'Error',
        text: formattedErrorMessage,
        confirmButtonColor: '#3085d6',
        confirmButtonText: 'OK'
    }).then(function () {
        window.location.href = '/CompanyAdmin/UserGroup';
    });
}

// This is for Update Group "GET" Method...
function editGroupUser(button) {
    var usergroupId = $(button).data('user-id');

    $('#editUserModal').find('#UserGroupId').val(usergroupId);
    $('#editUserModal').modal('show');
    $('.user-list1').empty();

    $.ajax({
        url: '/CompanyAdmin/UserGroup/GetGroupUser',
        type: 'GET',
        data: { id: usergroupId },
        success: function (response) {
            console.log(response);

            $('#groupError').text('');
            $('#userGroupError').text('');

            $('#userId').val(response.id);
            $('#groupname').val(response.groupName);

            var selectedUsernames = response.username.split(', ');

            fetchAllUsers(selectedUsernames);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });

    function fetchAllUsers(selectedUsernames) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/CompanyAdmin/UserGroup/UserName');
        xhr.onload = function () {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                console.log(response);

                var userList = response.userlist;
                var userListBox = $('.user-list1');

                userList.forEach(function (user, index) {
                    var checkboxId = 'userCheckbox' + user.value;
                    var checkbox = $('<div class="form-check checkbox-item"></div>');
                    var input = $('<input>', {
                        type: 'checkbox',
                        id: checkboxId,
                        value: user.value,
                        class: 'form-check-input',
                        name: 'users[]'
                    });
                    var label = $('<label>', {
                        for: checkboxId,
                        class: 'form-check-label',
                        text: user.text,
                        style: 'font-size: 18px;'
                    });

                    checkbox.append(input);
                    checkbox.append(label);
                    userListBox.append(checkbox)
                        ;

                    if (selectedUsernames.some(function (selectedUsername) {
                        return selectedUsername.toLowerCase() === user.text.toLowerCase();
                    })) {
                        input.prop('checked', true);
                    }

                    label.on('click', function (event) {
                        if (event.target !== input) {
                            event.preventDefault();
                        }
                    });

                    input.on('click', function (event) {
                        if (event.target === input && index === 0) {
                            event.preventDefault();
                        } else {
                            event.stopPropagation();
                        }
                    });
                });
            } else {
                console.log(xhr.statusText);
            }
        };
        xhr.onerror = function () {
            console.log(xhr.statusText);
        };
        xhr.send();
    }
}

// This is for Update Group "POST" Method....
$(document).ready(function () {
    $('#updategroupUserBtn').click(function (e) {
        e.preventDefault();

        var updateUserForm = $('#updateUserForm');
        var selectedUsers = $('.user-list1').find('input[type="checkbox"]:checked').map(function () {
            return $(this).siblings('label').text();
        }).get();

        $('.error-message').text('');

        var userGroupNameInput = $('#groupname');
        var userGroupName = userGroupNameInput.val().trim();
        var groupnameError = '';
        if (userGroupName === '') {
            groupnameError = '* Please enter Group name';
            userGroupNameInput.on('input', function () {
                if (userGroupNameInput.val().trim() !== '') {
                    $('#groupError').text('');
                }
            });
        }

        var usernameError = '';
        if (selectedUsers.length === 0) {
            usernameError = '* Please select at least one user';
        }

        $('#groupError').text(groupnameError);
        $('#userGroupError').text(usernameError);

        if (groupnameError || usernameError) {
            return;
        }

        var user = {
            UserGroupId: $('#UserGroupId').val(),
            UserGroupName: $('#groupname').val(),
            UserName: selectedUsers.join(", ")
        };

        $.ajax({
            url: updateUserForm.attr('action'),
            type: updateUserForm.attr('method'),
            data: JSON.stringify(user),
            contentType: 'application/json',
            success: function (response) {
                if (response) {
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


function showError(elementId, message) {
    $('#' + elementId).text(message);
}

    $('#updateUserForm').on('change', 'input[name="userList"]', function () {
        var usernameError = '';
        if ($('input[name="userList"]:checked').length === 0) {
            usernameError = '* Please select at least one user';
        }

        $('#userGroupError').text(usernameError);
    });
});

// Download All GropList in Excel Format...
function exportExcel() {
    window.location.href = '/CompanyAdmin/UserGroup/ExportExcel';
}

// Download All GropList in PDF Format...
function exportPdf() {
    debugger;
    window.location.href = '/CompanyAdmin/UserGroup/ExportPdf';
}