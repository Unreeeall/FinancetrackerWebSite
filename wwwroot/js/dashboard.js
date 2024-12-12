function addAccountWindow() {
    document.getElementById('new-acc-menu').classList.toggle("visible");


}

function closeAccountMenu() {
    document.getElementById('new-acc-menu').classList.toggle("visible");
}


function addTransactionWindow(button, event) {

    event.stopPropagation();
    document.getElementById('qick-add-transaction-container').classList.toggle("visible");
    const accountID = button.getAttribute('data-id');
    document.getElementById('account-id').value = accountID;
}

function closeTransactionWindow() {
    document.getElementById('qick-add-transaction-container').classList.toggle("visible");
}


function toggleImportField(event) {
    event.stopPropagation();
    document.getElementById('import-file-container').classList.toggle("visible");
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



$(document).ready(function () {
    $('.importForm').on('submit', function (event) {
        event.preventDefault(); // Prevent the default form submission
        var formData = new FormData(this); // Create FormData object
        var formId = $(this).attr('id'); // Get the form ID
        var resultId = '#result' //+ formId.split('-')[1]; // Generate the result div ID

        $.ajax({
            url: '/import-transactions', // Ensure this URL matches your route
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                $(resultId).html(
                    `<p>Transactions imported successfully.</p>
                    <p>Imported transactions: ${response.importedCount}</p>
                    <p>Skipped transactions: ${response.skippedCount}</p>`
                );
            },
            error: function (xhr, status, error) {
                $(resultId).html(`<p>An error occurred: ${xhr.responseText}</p>`);
            }
        });
    });
});








const accountDivs = document.getElementsByClassName('Acc-Container')
Array.prototype.forEach.call(accountDivs, element => {
    const uuid = element.getAttribute("uuid");
    element.addEventListener("click", event => {
        window.location.href = "AccountOverview?uuid=" + uuid;
    })
})




