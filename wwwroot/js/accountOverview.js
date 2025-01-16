document.addEventListener('DOMContentLoaded', (event) => {
    // Create the Chart.js charts.
    console.log("HI1");
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

    var ctx4 = document.getElementById('incomeExpensesChart').getContext('2d');
    var incomeExpensesChart = new Chart(ctx4, {
        type: 'line',
        data: {
            labels: [],
            datasets: [{
                label: 'Income',
                data: [],
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                borderWidth: 1,
                tension: 0.1 // This value controls the amount of curvature. 0 means no curve, 1 means maximum curve.
            },
            {
                label: 'Expense',
                data: [],
                borderColor: 'rgba(255, 99, 132, 1)',
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderWidth: 1,
                tension: 0.1 // This value controls the amount of curvature. 0 means no curve, 1 means maximum curve.
            }]
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    beginAtZero: false
                },
                y: {
                    beginAtZero: false
                }
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
        let weeklyIncomeData = [];
        let weeklyExpenseData = [];
        switch (timeframe) {
            case 'week':
                accountBalanceData = await fetchWeeklyData(date);
                expensesByCategoryData = await fetchExpenseData(date, timeframe);
                incomeByCategoryData = await fetchIncomeData(date, timeframe);
                weeklyIncomeData = await fetchWeeklyIncome(date);
                weeklyExpenseData = await fetchWeeklyExpense(date);
                break;
            case 'month':
                // Replace this with the actual monthly data fetching logic
                accountBalanceData = await fetchMonthlyData(date);
                expensesByCategoryData = await fetchExpenseData(date, timeframe);
                incomeByCategoryData = await fetchIncomeData(date, timeframe);
                weeklyIncomeData = await fetchMonthlyIncome(date);
                weeklyExpenseData = await fetchMonthlyExpense(date);
                break;
            case 'year':
                // Replace this with the actual yearly data fetching logic
                accountBalanceData = await fetchYearlyData(date);
                expensesByCategoryData = await fetchExpenseData(date, timeframe);
                incomeByCategoryData = await fetchIncomeData(date, timeframe);
                weeklyIncomeData = await fetchYearlylIncome(date);
                weeklyExpenseData = await fetchYearlyExpense(date);
                break;
            default:
                accountBalanceData = await fetchWeeklyData(date);
                expensesByCategoryData = await fetchExpenseData(date, "week");
                incomeByCategoryData = await fetchIncomeData(date, "week");
                weeklyIncomeData = await fetchWeeklyIncome(date);
                weeklyExpenseData = await fetchWeeklyExpense(date);
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

        incomeExpensesChart.data.labels = generateLabels(timeframe, date);
        incomeExpensesChart.data.datasets[0].data = weeklyIncomeData;
        incomeExpensesChart.data.datasets[1].data = weeklyExpenseData;
        incomeExpensesChart.update();
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
        updateIncomeAndExpense();
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
        updateIncomeAndExpense();
    });

    rangeTypeSelect.addEventListener('change', updateTimeframe);

    // Initialize timeframe display
    async function updateIncomeAndExpense() {
        const rangeType = rangeTypeSelect.value;
        const dateData = currentDate.toISOString();
        const accIDData = encodeURIComponent(accID);
        const userEmailData = encodeURIComponent(userEmail);

        try {
            const incomeResponse = await fetch(`api/Analysis/calculate-income?date=${dateData}&userEmail=${userEmailData}&accID=${accIDData}&rangeType=${rangeType}`);
            const incomeResponseData = await incomeResponse.json();
            document.getElementById('totalIncome').textContent = incomeResponseData.totalIncome;

            const expenseResponse = await fetch(`api/Analysis/calculate-expense?date=${dateData}&userEmail=${userEmailData}&accID=${accIDData}&rangeType=${rangeType}`);
            const expenseResponseData = await expenseResponse.json();
            document.getElementById('totalExpense').textContent = expenseResponseData.totalExpense;
        } catch (error) {
            console.error('Error fetching income or expense data:', error);
        }
    }

    // Initial call to set the income
    updateTimeframe();
    updateIncomeAndExpense();
    console.log("HI2");
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


async function fetchExpenseData(date, timeframe) {
    const response = await fetch(`api/Analysis/fetch-category-expense-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}&timeFrame=${timeframe}`);
    const CategoryExpenses = await response.json();
    console.log('Raw Expense by Category Data:', CategoryExpenses); // Log the raw response for debugging
    return CategoryExpenses;
}

async function fetchIncomeData(date, timeframe) {
    const response = await fetch(`api/Analysis/fetch-category-income-data?userEmail=${encodeURIComponent(userEmail)}&firstDateOfWeek=${date.toISOString()}&accID=${encodeURIComponent(accID)}&timeFrame=${timeframe}`);
    const CategoryIncome = await response.json();
    console.log('Raw Income by Category Data:', CategoryIncome); // Log the raw response for debugging
    return CategoryIncome;
}


async function fetchWeeklyIncome(date) {
    const response = await fetch(`api/Analysis/fetch-weekly-income-data?userEmail=${encodeURIComponent(userEmail)}&date=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    console.log(`Response Status: ${response.status}`);
    const dailyData = await response.json();
    console.log('Fetched Weekly Income Data:', dailyData);
    return dailyData;
}

async function fetchWeeklyExpense(date) {
    const response = await fetch(`api/Analysis/fetch-weekly-expense-data?userEmail=${encodeURIComponent(userEmail)}&date=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    console.log(`Response Status: ${response.status}`);
    const dailyData = await response.json();
    console.log('Fetched Weekly Income Data:', dailyData);
    return dailyData;
}

async function fetchMonthlyIncome(date) {
    const response = await fetch(`api/Analysis/fetch-monthly-income-data?userEmail=${encodeURIComponent(userEmail)}&date=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    console.log(`Response Status: ${response.status}`);
    const dailyData = await response.json();
    console.log('Fetched Weekly Income Data:', dailyData);
    return dailyData;
}

async function fetchMonthlyExpense(date) {
    const response = await fetch(`api/Analysis/fetch-monthly-expense-data?userEmail=${encodeURIComponent(userEmail)}&date=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    console.log(`Response Status: ${response.status}`);
    const dailyData = await response.json();
    console.log('Fetched Weekly Income Data:', dailyData);
    return dailyData;
}

async function fetchYearlylIncome(date) {
    const response = await fetch(`api/Analysis/fetch-yearly-income-data?userEmail=${encodeURIComponent(userEmail)}&date=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    console.log(`Response Status: ${response.status}`);
    const dailyData = await response.json();
    console.log('Fetched Weekly Income Data:', dailyData);
    return dailyData;
}

async function fetchYearlyExpense(date) {
    const response = await fetch(`api/Analysis/fetch-yearly-expense-data?userEmail=${encodeURIComponent(userEmail)}&date=${date.toISOString()}&accID=${encodeURIComponent(accID)}`);
    console.log(`Response Status: ${response.status}`);
    const dailyData = await response.json();
    console.log('Fetched Weekly Income Data:', dailyData);
    return dailyData;
}



function addTransactionWindow(button) {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('add-transaction-menu').classList.toggle("visible");
    const accountID = button.getAttribute('data-id');
    document.getElementById('account-id').value = accountID;
}

function closeTransactionWindow() {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('add-transaction-menu').classList.toggle("visible");
}

document.addEventListener("DOMContentLoaded", function () {
    const transactionTypeSelect = document.getElementById('trans-type-slct');
    const categorySelect = document.getElementById('trans-category-slct');
    const transferInputContainer = document.querySelector('.trans-transf-input-container');
    const contractCheckbox = document.getElementById('trans-contract-checkbox');
    const contractCycleDiv = document.querySelector('.trans-contract-cycle-container');
    const transferOriginSelect = document.getElementById('transf-orgini-slct');
    const transferDestinationSelect = document.getElementById('transf-destination-slct');

    // Function to filter categories based on transaction type
    function filterCategories() {
        const selectedType = transactionTypeSelect.value;
        const options = categorySelect.querySelectorAll('option');

        options.forEach(option => {
            if (option.value === "") {
                option.style.display = "block";
            } else {
                const dataType = option.getAttribute('data-type');
                if (selectedType === "Transfer") {
                    option.style.display = dataType === "Transfer" ? "block" : "none";
                } else {
                    option.style.display = dataType === selectedType ? "block" : "none";
                }
            }
        });
    }

    // Event listener for transaction type change
    transactionTypeSelect.addEventListener('change', function () {
        filterCategories();

        if (this.value === "Transfer") {
            transferInputContainer.style.display = "block";
        } else {
            transferInputContainer.style.display = "none";
            transferOriginSelect.value = ""; // Clear origin value if not Transfer
        }
    });

    // Event listener for contract checkbox change
    contractCheckbox.addEventListener('change', function () {
        if (this.checked) {
            contractCycleDiv.style.display = "block";
        } else {
            contractCycleDiv.style.display = "none";
        }
    });

    // Function to unselect the same option in the other select
    function unselectSameOption(event) {
        const originValue = transferOriginSelect.value;
        const destinationValue = transferDestinationSelect.value;

        if (event.target.id === 'transf-orgini-slct' && originValue === destinationValue) {
            transferDestinationSelect.value = "";
        } else if (event.target.id === 'transf-destination-slct' && originValue === destinationValue) {
            transferOriginSelect.value = "";
        }
    }

    // Event listeners for transfer selects
    transferOriginSelect.addEventListener('change', unselectSameOption);
    transferDestinationSelect.addEventListener('change', unselectSameOption);

    // Initialize the form based on default values
    filterCategories();
    transferInputContainer.style.display = transactionTypeSelect.value === "Transfer" ? "block" : "none";
    contractCycleDiv.style.display = contractCheckbox.checked ? "block" : "none";
});


function editTransactionWindow(event, button) {
    try {
        document.getElementById('dark-overlay').classList.toggle("visible");
        document.getElementById('edit-menu-container').classList.toggle("visible");
        const transactionID = button.getAttribute('data-id');
        const transactionType = button.getAttribute('data-type');
        const transactionCategory = button.getAttribute('data-category');
        var transactionAmount = button.getAttribute('data-amount');
        const transactionOrigin = button.getAttribute('data-origin');
        const transactionDestination = button.getAttribute('data-destination');
        const transactionDate = button.getAttribute('data-date');
        const transactionDescription = button.getAttribute('data-description');
        //const transactionIsContract = button.getAttribute('data-iscontract');

        
        // console.log("Transaction Amount (raw):", transactionAmount);
        // console.log("Parsed as float:", parseFloat(transactionAmount));
        
        document.getElementById('edit-trans-amount-inp').value = transactionAmount;
    


        document.getElementById('edit-Trans-ID').value = transactionID;
        document.getElementById('edit-type-select').value = transactionType;
        document.getElementById('edit-category-Dropdown').value = transactionCategory;
        // document.getElementById('edit-trans-amount-inp').value = transactionAmount;


        document.querySelector('.edit-trans-date-inp').value = transactionDate;

        document.getElementById('edit-trans-description-inp').value = transactionDescription;

        if (transactionType === "Transfer") {
            document.getElementById('edit-transf-orgin-slct').value = transactionOrigin;
            document.getElementById('edit-transf-destination-slct').value = transactionDestination;
            document.querySelector('.edit-trans-transf-input-container').style.display = "block";
        } else {
            document.querySelector('.edit-trans-transf-input-container').style.display = "none";
        }

        // document.getElementById('edit-trans-contract-checkbox').checked = transactionIsContract === "true";
        // document.querySelector('.edit-contract-cycle-slct').value = transactionIsContract === "true" ? transactionIsContract : "";

        // Show or hide contract cycle based on the contract checkbox
        // if (transactionIsContract === "true") {
        //     document.querySelector('.edit-trans-contract-cycle-container').style.display = "block";
        // } else {
        //     document.querySelector('.edit-trans-contract-cycle-container').style.display = "none";
        // }
    } catch (error) {
        console.error("Error setting transaction data: ", error);
    }
}

function closeEditMenu() {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('edit-menu-container').classList.toggle("visible");
}

document.addEventListener("DOMContentLoaded", function () {
    const edittransactionTypeSelect = document.getElementById('edit-type-select');
    const editcategorySelect = document.getElementById('edit-category-Dropdown');
    const edittransferInputContainer = document.querySelector('.edit-trans-transf-input-container');
    const editcontractCheckbox = document.getElementById('edit-trans-contract-checkbox');
    const editcontractCycleDiv = document.querySelector('.edit-trans-contract-cycle-container');
    const edittransferOriginSelect = document.getElementById('edit-transf-orgin-slct');
    const edittransferDestinationSelect = document.getElementById('edit-transf-destination-slct');

    // Function to filter categories based on transaction type
    function filterEditCategories() {
        const selectedType = edittransactionTypeSelect.value;
        const options = editcategorySelect.querySelectorAll('option');

        options.forEach(option => {
            if (option.value === "") {
                option.style.display = "block";
            } else {
                const dataType = option.getAttribute('data-type');
                if (selectedType === "Transfer") {
                    option.style.display = dataType === "Transfer" ? "block" : "none";
                } else {
                    option.style.display = dataType === selectedType ? "block" : "none";
                }
            }
        });
    }

    // Event listener for transaction type change
    edittransactionTypeSelect.addEventListener('change', function () {
        filterEditCategories();

        if (this.value === "Transfer") {
            edittransferInputContainer.style.display = "block";
        } else {
            edittransferInputContainer.style.display = "none";
            edittransferOriginSelect.value = ""; // Clear origin value if not Transfer
            edittransferDestinationSelect.value = ""; // Clear destination value if not Transfer
        }
    });

    // Event listener for contract checkbox change
    editcontractCheckbox.addEventListener('change', function () {
        if (this.checked) {
            editcontractCycleDiv.style.display = "block";
        } else {
            editcontractCycleDiv.style.display = "none";
        }
    });

    // Function to unselect the same option in the other select
    function unselectSameEditOption(event) {
        const originValue = edittransferOriginSelect.value;
        const destinationValue = edittransferDestinationSelect.value;

        if (event.target.id === 'edit-transf-orgin-slct' && originValue === destinationValue) {
            edittransferDestinationSelect.value = "";
        } else if (event.target.id === 'edit-transf-destination-slct' && originValue === destinationValue) {
            edittransferOriginSelect.value = "";
        }
    }

    // Event listeners for transfer selects
    edittransferOriginSelect.addEventListener('change', unselectSameEditOption);
    edittransferDestinationSelect.addEventListener('change', unselectSameEditOption);

    // Initialize the form based on default values
    filterEditCategories();
    edittransferInputContainer.style.display = edittransactionTypeSelect.value === "Transfer" ? "block" : "none";
    editcontractCycleDiv.style.display = editcontractCheckbox.checked ? "block" : "none";
});


function editContract(event, button) {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('edit-contract-container').classList.toggle("visible");

    const contractID = button.getAttribute('data-contractId');
    const contractType = button.getAttribute('data-type');
    const contractCategory = button.getAttribute('data-category');
    const contractAmount = button.getAttribute('data-amount');
    const contractOrigin = button.getAttribute('data-origin');
    const contractDestination = button.getAttribute('data-destination');
    const contractStartDate = button.getAttribute('data-startDate');
    const contractEndDate = button.getAttribute('data-endDate');
    const contractAccId = button.getAttribute('data-accountId');
    const contractCycle = button.getAttribute('data-cycle');

    console.log("ContractID: ", contractID);


    //document.getElementsByClassName('.contract-ID').value = contractID;
    document.getElementById('contract-id').value = contractID;
    document.getElementById('del-contract-id').value = contractID;
    document.getElementById('contract-type-slct').value = contractType;
    document.getElementById('contract-category-slct').value = contractCategory;
    document.getElementById('contract-amount-inp').value = contractAmount;
    document.getElementById('edit-contract-cycle-slct').value = contractCycle;
    document.getElementById('contract-acc-id').value = contractAccId;
    document.getElementById('del-contract-acc-id').value = contractAccId;

    // Set the date field first to ensure it always gets populated
    document.querySelector('.edit-start-date-input').value = contractStartDate;
    document.querySelector('.edit-end-date-input').value = contractEndDate;

    if (contractType === "Transfer") {
        document.getElementById('edit-contract-transf-orgin-slct').value = contractOrigin;
        document.getElementById('edit-contract-transf-destination-slct').value = contractDestination;
        document.querySelector('.edit-contract-transfer-input-container').style.display = "block";
    } else {
        document.querySelector('.edit-trans-transf-input-container').style.display = "none";
    }


    const testContractID = document.getElementById('del-contract-id').value;
    console.log("Test ContractID: ", testContractID);
}

function closeEditContract() {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('edit-contract-container').classList.toggle("visible");
}

// function editTransactionWindow(event, button) {
//     const editPopup = document.getElementById('edit-popup');
//     editPopup.classList.toggle("visible");

//     const fields = ['id', 'type', 'category', 'amount', 'origin', 'destination', 'date', 'description', 'iscontract', 'cycle'];
//     fields.forEach(field => {
//         const value = button.getAttribute(`data-${field}`);
//         const element = document.getElementById(`edit-${field.replace('id', 'Trans-ID')}`);
//         if (element) {
//             element.value = value;
//         }
//     });
// }

// function closeEditMenu() {
//     document.getElementById('edit-popup').classList.toggle("visible");
// }





