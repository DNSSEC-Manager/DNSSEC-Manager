var green = "40, 167, 69" //"#28a745"
var red = "220, 53, 69" //"#dc3545"
var yellow = "255, 193, 7" //"#ffc107"

$(document).ready(function () {
    var dataSignedChart = {
        labels: ['Signed', 'Unsigned'],
        datasets: [{
            label: 'Signed domains',
            data: [$('#signedChart').data('signed'), $('#signedChart').data('notsigned')],
            backgroundColor: [
                'rgba(' + green + ', 1)',
                'rgba(' + red + ', 1)'
            ],
            borderColor: [
                'rgba(' + green + ', 1)',
                'rgba(' + red + ', 1)'
            ],
            borderWidth: 1
        }]
    };
    var signedChartElem = document.getElementById('signedChart').getContext('2d');

    var signedChart = new Chart(signedChartElem, {
        type: 'pie',
        data: dataSignedChart,
        options: {
            maintainAspectRatio: false
        }
    });

    var dataActiveChart = {
        labels: ['Removed from DNS', 'Found', 'Not found'],
        datasets: [{
            label: 'Active domains',
            data: [$('#activeChart').data('removedfromdns'), $('#activeChart').data('active'), $('#activeChart').data('inactive')],
            backgroundColor: [
                'rgba(' + red + ', 1)',
                'rgba(' + green + ', 1)',
                'rgba(' + yellow + ', 1)'
            ],
            borderColor: [
                'rgba(' + red + ', 1)',
                'rgba(' + green + ', 1',
                'rgba(' + yellow + ', 1)'
            ],
            borderWidth: 1
        }]
    };
    var activeChartElem = document.getElementById('activeChart').getContext('2d');

    var activeChart = new Chart(activeChartElem, {
        type: 'pie',
        data: dataActiveChart,
        options: {
            maintainAspectRatio: false
        }
    });
});

//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIkRhc2hib2FyZC5qcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EiLCJmaWxlIjoiRGFzaGJvYXJkLmpzIiwic291cmNlc0NvbnRlbnQiOlsidmFyIGdyZWVuID0gXCI0MCwgMTY3LCA2OVwiIC8vXCIjMjhhNzQ1XCJcclxudmFyIHJlZCA9IFwiMjIwLCA1MywgNjlcIiAvL1wiI2RjMzU0NVwiXHJcbnZhciB5ZWxsb3cgPSBcIjI1NSwgMTkzLCA3XCIgLy9cIiNmZmMxMDdcIlxyXG5cclxuJChkb2N1bWVudCkucmVhZHkoZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIGRhdGFTaWduZWRDaGFydCA9IHtcclxuICAgICAgICBsYWJlbHM6IFsnU2lnbmVkJywgJ1Vuc2lnbmVkJ10sXHJcbiAgICAgICAgZGF0YXNldHM6IFt7XHJcbiAgICAgICAgICAgIGxhYmVsOiAnU2lnbmVkIGRvbWFpbnMnLFxyXG4gICAgICAgICAgICBkYXRhOiBbJCgnI3NpZ25lZENoYXJ0JykuZGF0YSgnc2lnbmVkJyksICQoJyNzaWduZWRDaGFydCcpLmRhdGEoJ25vdHNpZ25lZCcpXSxcclxuICAgICAgICAgICAgYmFja2dyb3VuZENvbG9yOiBbXHJcbiAgICAgICAgICAgICAgICAncmdiYSgnICsgZ3JlZW4gKyAnLCAxKScsXHJcbiAgICAgICAgICAgICAgICAncmdiYSgnICsgcmVkICsgJywgMSknXHJcbiAgICAgICAgICAgIF0sXHJcbiAgICAgICAgICAgIGJvcmRlckNvbG9yOiBbXHJcbiAgICAgICAgICAgICAgICAncmdiYSgnICsgZ3JlZW4gKyAnLCAxKScsXHJcbiAgICAgICAgICAgICAgICAncmdiYSgnICsgcmVkICsgJywgMSknXHJcbiAgICAgICAgICAgIF0sXHJcbiAgICAgICAgICAgIGJvcmRlcldpZHRoOiAxXHJcbiAgICAgICAgfV1cclxuICAgIH07XHJcbiAgICB2YXIgc2lnbmVkQ2hhcnRFbGVtID0gZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoJ3NpZ25lZENoYXJ0JykuZ2V0Q29udGV4dCgnMmQnKTtcclxuXHJcbiAgICB2YXIgc2lnbmVkQ2hhcnQgPSBuZXcgQ2hhcnQoc2lnbmVkQ2hhcnRFbGVtLCB7XHJcbiAgICAgICAgdHlwZTogJ3BpZScsXHJcbiAgICAgICAgZGF0YTogZGF0YVNpZ25lZENoYXJ0LFxyXG4gICAgICAgIG9wdGlvbnM6IHtcclxuICAgICAgICAgICAgbWFpbnRhaW5Bc3BlY3RSYXRpbzogZmFsc2VcclxuICAgICAgICB9XHJcbiAgICB9KTtcclxuXHJcbiAgICB2YXIgZGF0YUFjdGl2ZUNoYXJ0ID0ge1xyXG4gICAgICAgIGxhYmVsczogWydSZW1vdmVkIGZyb20gRE5TJywgJ0ZvdW5kJywgJ05vdCBmb3VuZCddLFxyXG4gICAgICAgIGRhdGFzZXRzOiBbe1xyXG4gICAgICAgICAgICBsYWJlbDogJ0FjdGl2ZSBkb21haW5zJyxcclxuICAgICAgICAgICAgZGF0YTogWyQoJyNhY3RpdmVDaGFydCcpLmRhdGEoJ3JlbW92ZWRmcm9tZG5zJyksICQoJyNhY3RpdmVDaGFydCcpLmRhdGEoJ2FjdGl2ZScpLCAkKCcjYWN0aXZlQ2hhcnQnKS5kYXRhKCdpbmFjdGl2ZScpXSxcclxuICAgICAgICAgICAgYmFja2dyb3VuZENvbG9yOiBbXHJcbiAgICAgICAgICAgICAgICAncmdiYSgnICsgcmVkICsgJywgMSknLFxyXG4gICAgICAgICAgICAgICAgJ3JnYmEoJyArIGdyZWVuICsgJywgMSknLFxyXG4gICAgICAgICAgICAgICAgJ3JnYmEoJyArIHllbGxvdyArICcsIDEpJ1xyXG4gICAgICAgICAgICBdLFxyXG4gICAgICAgICAgICBib3JkZXJDb2xvcjogW1xyXG4gICAgICAgICAgICAgICAgJ3JnYmEoJyArIHJlZCArICcsIDEpJyxcclxuICAgICAgICAgICAgICAgICdyZ2JhKCcgKyBncmVlbiArICcsIDEnLFxyXG4gICAgICAgICAgICAgICAgJ3JnYmEoJyArIHllbGxvdyArICcsIDEpJ1xyXG4gICAgICAgICAgICBdLFxyXG4gICAgICAgICAgICBib3JkZXJXaWR0aDogMVxyXG4gICAgICAgIH1dXHJcbiAgICB9O1xyXG4gICAgdmFyIGFjdGl2ZUNoYXJ0RWxlbSA9IGRvY3VtZW50LmdldEVsZW1lbnRCeUlkKCdhY3RpdmVDaGFydCcpLmdldENvbnRleHQoJzJkJyk7XHJcblxyXG4gICAgdmFyIGFjdGl2ZUNoYXJ0ID0gbmV3IENoYXJ0KGFjdGl2ZUNoYXJ0RWxlbSwge1xyXG4gICAgICAgIHR5cGU6ICdwaWUnLFxyXG4gICAgICAgIGRhdGE6IGRhdGFBY3RpdmVDaGFydCxcclxuICAgICAgICBvcHRpb25zOiB7XHJcbiAgICAgICAgICAgIG1haW50YWluQXNwZWN0UmF0aW86IGZhbHNlXHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0pO1xyXG4iXX0=
