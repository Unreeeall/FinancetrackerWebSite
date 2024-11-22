// async function expenseChart() {
//     var response = await fetch("/api/Transaction");
//     if (!response.ok) {
//         return;
//     }
//     // var transactionlist = [];
//     var transactionjson = await response.json();
//     transactionjson.array.forEach(element => {
//         // transactionlist.push(element["Category"])
//         var category = element["Category"];
//         if (categoryAmount[category]) {
//             categoryAmount[category] += amount;
//         } else {
//             categoryAmount[category] = amount;
//         }
//     });

//     var labels = Object.keys(categoryAmount);
//     var data = Object.values(categoryAmount);

//     var expenseDataDougnutChart = {
//         labels: labels,
//         datasets: [{
//             data: data,
//             backgroundColor: [
//                 'rgba(255, 99, 132, 0.2)',
//                 'rgba(200, 100, 50, 0.2)',
//                 'rgba(255, 206, 86, 0.2)',
//                 'rgba(75, 192, 192, 0.2)',
//                 'rgba(153, 102, 255, 0.2)',
//                 'rgba(255, 159, 64, 0.2)'
//             ],
//             borderColor: [
//                 'rgba(255, 99, 132, 1)',
//                 'rgba(54, 162, 235, 1)',
//                 'rgba(255, 206, 86, 1)',
//                 'rgba(75, 192, 192, 1)',
//                 'rgba(153, 102, 255, 1)',
//                 'rgba(255, 159, 64, 1)'
//             ],
//             borderWidth: 2,
//             hoverOffset: 16
//         }]
//     }

//     var ctx = document.getElementById('budgetPieChart').getContext('2d');
//     var budgetPieChart = new Chart(ctx, {
//         type: 'doughnut',
//         data: expenseDataDougnutChart,

//         // labels: ['Savings', 'Shopping', 'Food & Drinks', 'Car', 'Entertainment', 'Miscellaneous'], * @
//         // datasets:  
//         // label: 'Spendings in %',


//         options: {
//             responsive: true,
//             maintainAspectRatio: true,

//             Animation: {
//                 animateScale: true,
//                 animateRotate: true
//             },
//             layout: {
//                 padding: 25
//             },
//             plugins: {
//                 legend: {
//                     display: true,
//                     position: 'bottom',
//                     labels: {
//                         color: '#ffffff'
//                     }
//                 }
//             }
//         }

//     });
// }


// function investmentChart()
// {
//     var ctx = document.getElementById('investmentLineChart').getContext('2d');
//         var investmentLineChart = new Chart(ctx, {
//             type: 'line',
//             data: {
//                 labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
//                 datasets: [{
//                     label: 'Invested Capital',
//                     data: [10, 20, 30, 40, 50, 60, 70],
//                     borderColor: 'rgba(75, 192, 192, 1)',
//                     backgroundColor: 'rgba(75, 192, 192, 0.2)',
//                     fill: true
//                 },
//                 {
//                     label: 'Worth with Win Margin',
//                     data: [15, 25, 35, 50, 65, 80, 100], // Example data
//                     borderColor: 'rgba(153, 102, 255, 1)',
//                     backgroundColor: 'rgba(153, 102, 255, 0.2)',
//                     fill: true
//                 }]
//             },
//             options: {
//                 responsive: true,
//                 maintainAspectRatio: false,
//                 plugins: {
//                     legend: {
//                         display: true,
//                         position: 'bottom',
//                         labels: {
//                             color: '#ffffff'
//                         }
//                     },
//                     scales: {
//                         x: {
//                             ticks: {
//                                 color: '#ffffff'
//                             }
//                         },
//                         y: {
//                             ticks: {
//                                 color: '#ffffff'
//                             }
//                         }
//                     }
//                 }
//             }
//         });
// }


async function expenseChart() {
    var response = await fetch("/api/Transaction");
    if (!response.ok) {
        return;
    }
    var transactionjson = await response.json();

    // Initialize the categoryAmount object
    var categoryAmount = {};

    // Iterate over the transaction data
    transactionjson.forEach(element => {
        var type = element["Type"];
        var category = element["Category"];
        var amount = parseFloat(element["Amount"]); // Ensure the amount is a number

        if (type === "Expense") {
            if (categoryAmount[category]) {
                categoryAmount[category] += amount;
            } else {
                categoryAmount[category] = amount;
            }
        }
    });

    // Prepare data for doughnut chart
    var labels = Object.keys(categoryAmount);
    var data = Object.values(categoryAmount);

    var expenseDataDoughnutChart = {
        labels: labels,
        datasets: [{
            data: data,
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(200, 100, 50, 0.2)',
                'rgba(255, 206, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(255, 159, 64, 0.2)'
            ],
            borderColor: [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(153, 102, 255, 1)',
                'rgba(255, 159, 64, 1)'
            ],
            borderWidth: 2,
            hoverOffset: 16
        }]
    };

    var ctx = document.getElementById('expensePieChart').getContext('2d');
    var expensePieChart = new Chart(ctx, {
        type: 'doughnut',
        data: expenseDataDoughnutChart,
        options: {
            responsive: true,
            maintainAspectRatio: true,
            animation: {
                animateScale: true,
                animateRotate: true
            },
            layout: {
                padding: 25
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        color: '#ffffff'
                    }
                }
            }
        }
    });
}

function investmentChart() {
    var ctx = document.getElementById('investmentLineChart').getContext('2d');
    var investmentLineChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
            datasets: [{
                label: 'Invested Capital',
                data: [10, 20, 30, 40, 50, 60, 70],
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                fill: true
            },
            {
                label: 'Worth with Win Margin',
                data: [15, 25, 35, 50, 65, 80, 100], // Example data
                borderColor: 'rgba(153, 102, 255, 1)',
                backgroundColor: 'rgba(153, 102, 255, 0.2)',
                fill: true
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        color: '#ffffff'
                    }
                },
                scales: {
                    x: {
                        ticks: {
                            color: '#ffffff'
                        }
                    },
                    y: {
                        ticks: {
                            color: '#ffffff'
                        }
                    }
                }
            }
        }
    });
}

async function budgetChart() {
    var response = await fetch("/api/Transaction");
    if (!response.ok) {
        console.error('Failed to fetch transactions');
        return;
    }
    var transactions = await response.json();

    function formatDate(dateString) {
        const date = new Date(dateString);
        return date.toISOString().split('T')[0];
    }

    function aggregateData(transactions) {
        const result = {
            Expense: {},
            Income: {}
        };

        transactions.forEach(transaction => {
            const date = formatDate(transaction.Date); // Ensure the correct property name
            const type = transaction.Type; // Ensure the correct property name
            const amount = parseFloat(transaction.Amount); // Ensure amount is parsed correctly

            if (!result[type][date]) {
                result[type][date] = 0;
            }
            result[type][date] += amount;
        });

        return result;
    }

    const aggregatedData = aggregateData(transactions);

    function convertToChartData(aggregatedData) {
        const expenseData = [];
        const incomeData = [];

        for (const date in aggregatedData.Expense) {
            expenseData.push({ x: date, y: aggregatedData.Expense[date] });
        }

        for (const date in aggregatedData.Income) {
            incomeData.push({ x: date, y: aggregatedData.Income[date] });
        }

        return { expenseData, incomeData };
    }

    const chartData = convertToChartData(aggregatedData);

    const hasExpenseData = chartData.expenseData.length > 0;
    const hasIncomeData = chartData.incomeData.length > 0;

    const expenseData = hasExpenseData ? chartData.expenseData : [{ x: new Date(), y: 0 }];
    const incomeData = hasIncomeData ? chartData.incomeData : [{ x: new Date(), y: 0 }];

    // Debugging logs
    console.log('Aggregated Data:', aggregatedData);
    console.log('Chart Data:', chartData);

    var ctx = document.getElementById('mixedChart').getContext('2d');
    var mixedChart = new Chart(ctx, {
        type: 'bar', // base type for the chart
        data: {
            datasets: [{
                label: 'Expenses',
                data: expenseData,
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1
            }, {
                label: 'Income',
                data: incomeData,
                type: 'line', // specify the dataset type as line
                borderColor: 'rgba(255, 99, 132, 1)',
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                fill: false
            }]
        },
        options: {
            scales: {
                x: {
                    type: 'time',
                    time: {
                        unit: 'day'
                    },
                    title: {
                        display: true,
                        text: 'Date'
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Amount'
                    }
                }
            }
        }
    });
}


// Call the function to fetch data and create the chart after DOM is fully loaded
document.addEventListener('DOMContentLoaded', () => {
    expenseChart();
    investmentChart();
    budgetChart();
});




function toggleMenu() {
    const menu = document.getElementById('menu');
    const menuIcon = document.querySelector('.menu-icon');
    menu.classList.toggle('show');
    menuIcon.classList.toggle('active');
    adjustChartPosition();
}

function adjustChartPosition() {
    const budgetChart = document.querySelector('.Budget-Chart');
    const investmentChart = document.querySelector('.Investment-Chart');
    const transactionChart = document.querySelector('.Transaction-History-Chart');
    const menu = document.getElementById('menu');
    if (menu.classList.contains('show')) {
        budgetChart.style.marginRight = '0';
        investmentChart.style.marginLeft = '0';
    } else {
        budgetChart.style.marginRight = '20px';
        investmentChart.style.marginLeft = '20px';
    }
}




// Adjust chart position on window resize
window.addEventListener('resize', adjustChartPosition);

// Initial adjustment
adjustChartPosition();










