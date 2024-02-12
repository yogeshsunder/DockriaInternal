var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    debugger;
    dataTable = $('#tblDataTable').DataTable({
        "ajax": {
            "url": "/Admin/MasterAdmin/GetAll",
            "type": "GET",
            "datatype": "json",
            "dataSrc": "data"
        },
        "columns": [
            {
                "data": "date",
                "width": "10%",
                "render": function (data) {
                    const dateObj = new Date(data);
                    const formattedDate = `${dateObj.getDate()}/${dateObj.getMonth() + 1}/${dateObj.getFullYear()}`;
                    return formattedDate;
                }
            },
            {
                "data": "time",
                "width": "10%",
                "render": function (data) {
                    const timeParts = data.split(":");
                    const formattedTime = `${timeParts[0]}:${timeParts[1]}:${Math.floor(parseFloat(timeParts[2]))}`;
                    return formattedTime;
                }
            },
            { "data": "username", "width": "10%" },
            { "data": "fullname", "width": "10%" },
            {
                "data": "browserName",
                "width": "10%",
                "render": function (data, type, row) {
                    if (type === "display") {
                        if (data === null) {
                            return ""; // Return an empty string for null values
                        }

                        if (data.length > 15) {
                            return `<span title="${data}">${data.substr(0, 15)}...</span>`;
                        } else {
                            return data;
                        }
                    }
                    return data;

                }
            },
            { "data": "ipAddress", "width": "10%" },
            /*{ "data": "action", "width": "10%" },*/
            { "data": "recordType", "width": "10%" },
         /*   { "data": "oldData", "width": "10%" },*/
            { "data": "newData", "width": "10%" },
           /* { "data": "description", "width": "10%" },*/

        ],
        "drawCallback": function () {
            $('#tblDataTable td').addClass('border');
        }
    });
}

// Get the canvas element
const ctx = document.getElementById('myChart').getContext('2d');

// Create the chart
const myChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple'],
        datasets: [{
            label: 'Sample Dataset',
            data: [12, 19, 3, 5, 2, 3],
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1
        }]
    },
    options: {
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});

// Call loadAuditLogAlerts on page load or when admin dashboard is accessed
$(document).ready(function () {

    loadAuditLogAlerts();
});

var isScrolling = true; // Flag to track scrolling state

function loadAuditLogAlerts() {
    var logs = []; // Array to store the logs

    setInterval(function () {
        // Make an AJAX request to fetch the audit log data
        $.ajax({
            url: '/Admin/MasterAdmin/GetAuditLogs',
            method: 'GET',
            success: function (response) {
                // Filter the logs to get only the ones from the last ten seconds
                var currentTime = new Date();
                var tenSecondsAgo = currentTime - 10000; // Subtract 10 seconds in milliseconds
                var filteredLogs = response.filter(function (log) {
                    var logTimestamp = new Date(log.timestamp);
                    return logTimestamp >= tenSecondsAgo && logTimestamp <= currentTime;
                });

                // Update the logs array with the filtered logs
                logs = filteredLogs;

                // Extract the relevant information from the logs
                var logTexts = logs.map(function (log) {
                    return log.actionType + ' by ' + log.user + ' at ' + log.timestamp;
                });

                // Update the words array with the log texts
                words = logTexts;

                // Start the word flicker animation
                wordFlick();

                // Start or stop the scrolling animation based on the isScrolling flag
                if (isScrolling) {
                    startScrolling();
                } else {
                    stopScrolling();
                }
            }
        });
    }, 10000); // Fetch the latest audit log every 10 seconds (adjust the interval as needed)
}

// Rest of the code

function startScrolling() {
    // Get the audit log display element
    var auditLogDisplay = document.getElementById('auditLogDisplay');

    // Calculate the animation duration based on the width and desired speed
    var displayWidth = auditLogDisplay.offsetWidth;
    var animationDuration = displayWidth / 50; // Adjust the divisor for speed control

    // Apply the animation duration
    auditLogDisplay.style.animationDuration = animationDuration + 's';
}

function stopScrolling() {
    // Get the audit log display element
    var auditLogDisplay = document.getElementById('auditLogDisplay');

    // Remove the scrolling-text class
    auditLogDisplay.classList.remove('scrolling-text');
}

function wordFlick() {
    var currentIndex = 0;
    var part;
    var offset = 0;
    var len = words.length;
    var forwards = true;
    var skip_count = 0;
    var skip_delay = 15;
    var speed = 40;
    var pauseDuration = 2000; // Duration to show the complete log list before stopping in milliseconds

    var intervalId;

    function scrollNextLog() {
        if (forwards) {
            if (offset >= words[currentIndex].length) {
                ++skip_count;
                if (skip_count == skip_delay) {
                    forwards = false;
                    skip_count = 0;
                }
            }
        } else {
            if (offset == 0) {
                forwards = true;
                currentIndex++;
                offset = 0;
                if (currentIndex >= len - 1) { // Update the condition to stop before the last log text
                    stopScrollingAnimation();
                    return;
                }
            }
        }
        part = words[currentIndex].substr(0, offset);
        if (skip_count == 0) {
            if (forwards) {
                offset++;
            } else {
                offset--;
            }
        }

        if (currentIndex === len - 1 && offset === words[currentIndex].length && !forwards) { // Update the condition to exclude the last alphabet
            $('#wordContainer').text(''); // Clear the text when scrolling stops
        } else {
            $('#wordContainer').text(part);
            $('#wordContainer').toggleClass('text-warning', forwards); // Add or remove the 'text-warning' class based on the forwards variable
        }

        intervalId = setTimeout(scrollNextLog, speed);
    }

    scrollNextLog();

    // Stop the scrolling animation
    function stopScrollingAnimation() {
        debugger;
        clearTimeout(intervalId);
        stopScrolling();
    }
}    
