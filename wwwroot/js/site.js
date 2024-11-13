
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




