document.addEventListener('DOMContentLoaded', (event) => {
    // Create the Chart.js charts.
    const ctx1 = document.getElementById('accountBalanceChart').getContext('2d');
    const accountBalanceChart = new Chart(ctx1, {
        type: 'line',
        data: {
            labels: [], // These labels will be dynamically updated.
            datasets: [{
                label: 'Account Balance',
                data: [],
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                borderWidth: 1,
                tension: 0.1 // This value controls the amount of curvature. 0 means no curve, 1 means maximum curve.
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: false
                }
            }
        }
    });

    const ctx2 = document.getElementById('expensesByCategoryChart').getContext('2d');
    const expensesByChategoryChart = new Chart(ctx2, {
        type: 'doughnut',
        data: {
            labels: [],
            datasets: [{
                data: [],
                backgroundColor: dynamicColors,
                borderColor: dynamicBorderColors,
                borderWidth: 3,
                hoverOffset: 16
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false, // Allow flexibility in aspect ratio
            layout: {
                padding: {
                    top: 20,
                    bottom: 20,
                    left: 50,
                    right: 50
                }
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        color: '#ffffff'
                    }
                }
            },
            animation: {
                animateScale: true,
                animateRotate: true
            }
        }
    });


    const ctx3 = document.getElementById('incomeByCategoryChart').getContext('2d');
    const incomeByChategoryChart = new Chart(ctx3, {
        type: 'doughnut',
        data: {
            labels: [],
            datasets: [{
                data: [],
                backgroundColor: dynamicColors,
                borderColor: dynamicBorderColors,
                borderWidth: 3,
                hoverOffset: 16
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false, // Allow flexibility in aspect ratio
            layout: {
                padding: {
                    top: 20,
                    bottom: 20,
                    left: 50,
                    right: 50
                }
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        color: '#ffffff'
                    }
                }
            },
            animation: {
                animateScale: true,
                animateRotate: true
            }
        }
    });



    // Function to generate date labels for the selected timeframe.
    function generateLabels(timeframe, date) {
        let labels = [];
        const options = { month: 'short', day: 'numeric' };

        switch (timeframe) {
            case 'week':
                const startOfWeek = new Date(date);
                // startOfWeek.setDate(date.getDate() - date.getDay());
                for (let i = 0; i < 7; i++) {
                    let currentDate = new Date(startOfWeek);
                    currentDate.setDate(startOfWeek.getDate() + i);
                    labels.push(currentDate.toLocaleDateString(undefined, options));
                }
                break;
            case 'month':
                const daysInMonth = new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate();
                for (let i = 1; i <= daysInMonth; i++) {
                    labels.push(new Date(date.getFullYear(), date.getMonth(), i).toLocaleDateString(undefined, options));
                }
                break;
            case 'year':
                labels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                break;
            default:
                break;
        }
        return labels;
    }

    // Function to update the chart based on selected timeframe.
    async function updateChartData(timeframe, date) {
        let accountBalanceData = [];
        let expensesByCategoryData = { categories: [], expenses: [] };
        let incomeByCategoryData = { categories: [], expenses: [] };
        switch (timeframe) {
            case 'week':
                accountBalanceData = await fetchWeeklyData(date);
                expensesByCategoryData = await fetchWeeklyExpenseData(date);
                incomeByCategoryData = await fetchWeeklyIncomeData(date);
                break;
            case 'month':
                // Replace this with the actual monthly data fetching logic
                accountBalanceData = await fetchMonthlyData(date);
                expensesByCategoryData = await fetchMonthlyExpenseData(date);
                incomeByCategoryData = await fetchMonthlyIncomeData(date);
                break;
            case 'year':
                // Replace this with the actual yearly data fetching logic
                accountBalanceData = await fetchYearlyData(date);
                expensesByCategoryData  = await fetchYearlyExpenseData(date);
                incomeByCategoryData = await fetchYearlyIncomeData(date);
                break;
            default:
                accountBalanceData = await fetchWeeklyData(date);
                expensesByCategoryData = await fetchWeeklyExpenseData(date);
                incomeByCategoryData = await fetchWeeklyIncomeData(date);
        }

        console.log('Account Balance Data:', accountBalanceData);
        console.log('Expenses By Category Data:', expensesByCategoryData);
        console.log('Income By Category Data:', incomeByCategoryData);


        accountBalanceChart.data.labels = generateLabels(timeframe, date);
        accountBalanceChart.data.datasets[0].data = accountBalanceData;
        accountBalanceChart.update();

        expensesByChategoryChart.data.labels = expensesByCategoryData.categories || [];
        expensesByChategoryChart.data.datasets[0].data = expensesByCategoryData.expenses || [];
        expensesByChategoryChart.update();

        incomeByChategoryChart.data.labels = incomeByCategoryData.categories || [];
        incomeByChategoryChart.data.datasets[0].data = incomeByCategoryData.expenses || [];
        incomeByChategoryChart.update();
    }

    // Timeframe management logic
    const timeframeSpan = document.getElementById('timeframe');
    const rangeTypeSelect = document.getElementById('rangeType');
    let date = new Date();
    var day = date.getDay(),
        diff = date.getDate() - day + (day == 0 ? -6 : 1); // adjust when day is sunday
    let currentDate = new Date(date.setDate(diff));


    function formatTimeframe(date, rangeType) {
        const options = { year: 'numeric', month: 'short', day: 'numeric' };
        switch (rangeType) {
            case 'week':
                const startOfWeek = date;
                // startOfWeek.setDate(date.getDate() - date.getDay());
                const endOfWeek = new Date(startOfWeek);
                endOfWeek.setDate(startOfWeek.getDate() + 6);
                return `${startOfWeek.toLocaleDateString(undefined, options)} - ${endOfWeek.toLocaleDateString(undefined, options)}`;
            case 'month':
                return date.toLocaleDateString(undefined, { year: 'numeric', month: 'long' });
            case 'year':
                return date.getFullYear();
            default:
                return date.toLocaleDateString();
        }
    }

    function updateTimeframe() {
        const rangeType = rangeTypeSelect.value;
        timeframeSpan.textContent = formatTimeframe(currentDate, rangeType);
        updateChartData(rangeType, currentDate); // Update the chart data based on selected timeframe.
    }

    document.getElementById('prev').addEventListener('click', () => {
        const rangeType = rangeTypeSelect.value;
        if (rangeType === 'week') {
            currentDate.setDate(currentDate.getDate() - 7);
        } else if (rangeType === 'month') {
            currentDate.setMonth(currentDate.getMonth() - 1);
        } else if (rangeType === 'year') {
            currentDate.setFullYear(currentDate.getFullYear() - 1);
        }
        updateTimeframe();
    });

    document.getElementById('next').addEventListener('click', () => {
        const rangeType = rangeTypeSelect.value;
        if (rangeType === 'week') {
            currentDate.setDate(currentDate.getDate() + 7);
        } else if (rangeType === 'month') {
            currentDate.setMonth(currentDate.getMonth() + 1);
        } else if (rangeType === 'year') {
            currentDate.setFullYear(currentDate.getFullYear() + 1);
        }
        updateTimeframe();
    });

    rangeTypeSelect.addEventListener('change', updateTimeframe);

    // Initialize timeframe display
    updateTimeframe();
});



function generateModernColors(numColors) {
    const baseHues = [200, 300]; // Base hues for purple and blue-ish tones
    const colors = [];
    for (let i = 0; i < numColors; i++) {
        const hue = baseHues[i % baseHues.length] + Math.floor((i / baseHues.length) * 45) % 360;
        const color = `hsl(${hue}, 70%, 50%)`; // Darker colors with 35% lightness
        colors.push(color);
    }
    return colors;
}

const numCategories = 20; // Example number of categories
const dynamicColors = generateModernColors(numCategories);
const dynamicBorderColors = dynamicColors.map(color => color.replace('50%', '35%')); // Darker border colors









async function fetchWeeklyData(date) {
    const response = await fetch(`api/Analysis/fetch-weekly-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    const weeklyData = await response.json();
    return weeklyData;
}

async function fetchMonthlyData(date) {
    const response = await fetch(`api/Analysis/fetch-monthly-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    const monthlyData = await response.json();
    return monthlyData;
}

async function fetchYearlyData(date) {
    const response = await fetch(`api/Analysis/fetch-yearly-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    const yearlyData = await response.json();
    return yearlyData;
}


async function fetchWeeklyExpenseData(date) {
    const response = await fetch(`api/Analysis/fetch-weekly-category-expense-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    const weeklyCategorieExepnses = await response.json();
    console.log('Raw Weekly Expense Data:', weeklyCategorieExepnses); // Log the raw response for debugging
    return weeklyCategorieExepnses;
}

async function fetchWeeklyIncomeData(date) {
    const response = await fetch(`api/Analysis/fetch-weekly-category-income-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    const weeklyCategoryIncome = await response.json();
    console.log('Raw Weekly Income Data:', weeklyCategoryIncome); // Log the raw response for debugging
    return weeklyCategoryIncome;
}

async function fetchMonthlyExpenseData(date) {
    const response = await fetch(`api/Analysis/fetch-monthly-category-expense-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    const monthlyCategoryExpense = await response.json();
    console.log('Raw Monthly Income Data:', monthlyCategoryExpense); // Log the raw response for debugging
    return monthlyCategoryExpense;
}

async function fetchMonthlyIncomeData(date) {
    const response = await fetch(`api/Analysis/fetch-monthly-category-income-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    const monthlyCategoryIncome = await response.json();
    console.log('Raw Monthly Income Data:', monthlyCategoryIncome); // Log the raw response for debugging
    return monthlyCategoryIncome;
}


async function fetchYearlyExpenseData(date) {
    const response = await fetch(`api/Analysis/fetch-yearly-category-expense-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    const yearlyCategoryExpense = await response.json();
    console.log('Raw Yearly Income Data:', yearlyCategoryExpense); // Log the raw response for debugging
    return yearlyCategoryExpense;
}

async function fetchYearlyIncomeData(date) {
    const response = await fetch(`api/Analysis/fetch-yearly-category-income-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    const yearlyCategoryIncome = await response.json();
    console.log('Raw Yearly Income Data:', yearlyCategoryIncome); // Log the raw response for debugging
    return yearlyCategoryIncome;
}


