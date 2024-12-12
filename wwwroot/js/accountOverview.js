document.addEventListener('DOMContentLoaded', (event) => {
    // Sample data for demonstration; you can replace these with dynamic values.
    const sampleData = {
        weekly: [100, 200, 150, 300, 250, 400, 350], // Data for 7 days
        monthly: Array.from({ length: 31 }, (_, i) => Math.random() * 1000), // Data for maximum 31 days
        yearly: Array.from({ length: 12 }, (_, i) => Math.random() * 10000) // Data for 12 months
    };

    // Create the Chart.js chart.
    const ctx = document.getElementById('accountBalanceChart').getContext('2d');
    const accountBalanceChart = new Chart(ctx, {
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

    // Function to generate date labels for the selected timeframe.
    function generateLabels(timeframe, date) {
        let labels = [];
        const options = { month: 'short', day: 'numeric' };

        switch (timeframe) {
            case 'week':
                const startOfWeek = new Date(date);
                startOfWeek.setDate(date.getDate() - date.getDay());
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
    function updateChartData(timeframe, date) {
        let data = [];
        switch (timeframe) {
            case 'week':
                data = sampleData.weekly;
                break;
            case 'month':
                data = sampleData.monthly.slice(0, new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate());
                break;
            case 'year':
                data = sampleData.yearly;
                break;
            default:
                data = sampleData.weekly;
        }
        accountBalanceChart.data.labels = generateLabels(timeframe, date);
        accountBalanceChart.data.datasets[0].data = data;
        accountBalanceChart.update();
    }

    // Timeframe management logic
    const timeframeSpan = document.getElementById('timeframe');
    const rangeTypeSelect = document.getElementById('rangeType');
    let currentDate = new Date();

    function formatTimeframe(date, rangeType) {
        const options = { year: 'numeric', month: 'short', day: 'numeric' };
        switch (rangeType) {
            case 'week':
                const startOfWeek = new Date(date);
                startOfWeek.setDate(date.getDate() - date.getDay());
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