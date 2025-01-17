function addAccountWindow() {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('add-new-acc-menu').classList.toggle("visible");
}

function closeAccountMenu() {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('add-new-acc-menu').classList.toggle("visible");
}


function addTransactionWindow(button, event) {

    event.stopPropagation();
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('add-transaction-menu').classList.toggle("visible");
    const accountID = button.getAttribute('data-id');
    document.getElementById('account-id').value = accountID;
}

function closeTransactionWindow() {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('add-transaction-menu').classList.toggle("visible");
}


function openImportField(event) {
    event.stopPropagation();
    document.getElementById('import-file-container').classList.toggle("visible");
    document.getElementById('dark-overlay').classList.toggle("visible");
}

function closeImportField() {

    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('import-file-container').classList.toggle("visible");
}

function showAlert() {
    const type = document.getElementById('trans-type-slct').value;
    const category = document.getElementById('trans-category-slct').value;
    const transfOrigin = document.getElementById('transf-orgini-slct').value;
    const transfDestination = document.getElementById('transf-destination-slct').value;
    const description = document.getElementById('trans-description-inp').value;
    const amount = document.getElementById('trans-amount-inp').value;
    const date = document.getElementById('trans-date-inp').value;
    const isContract = document.getElementById('trans-contract-checkbox').checked;
    const cycle = document.getElementById('contract-cycle-slct').value;

    const baseFieldsFilled = (type != "") && (category != "") && (description != "") && (amount != "") && (date != "");
    const contractFieldsFilled = isContract && cycle != "";
    const transferFieldsFilled = transfOrigin != "" && transfDestination != "";

    console.log(baseFieldsFilled, contractFieldsFilled, transferFieldsFilled);

    if(type != "Transfer"){
        if(!baseFieldsFilled){
            alert("Please fillout all fields!");
            return false;
        }
        else if(baseFieldsFilled && !contractFieldsFilled){
            alert("Please fillout all fields!");
            return false;
        }
        else{
            if(type === "Expense"){
                alert("Expense added successfully!");
                return true;
            }
            else if(type === "Income"){
                alert("Income added successfully!");
                return true;
            }
            else if(type === "Transfer"){
                alert("Transfer added successfully!");
                return true;
            }
            else {
                alert("Error. Unknown Transaction Type");
                return true;
            }
        }
    }
    else{
        if(!transferFieldsFilled){
            alert("Please fillout all fields!");
            return false;
        }
        else{
            if(type === "Expense"){
                alert("Expense added successfully!");
                return true;
            }
            else if(type === "Income"){
                alert("Income added successfully!");
                return true;
            }
            else if(type === "Transfer"){
                alert("Transfer added successfully!");
                return true;
            }
            else {
                alert("Error. Unknown Transaction Type");
                return true;
            }
        }
    }
    
    
    


    // if(type != null && category != null && description != null && amount != null && date != null){
    //     if(isContract && cycle == null){
    //         alert("Please fillout all fields!");
    //         return false;
    //     }

    //     if(type === "Expense"){
    //         alert("Expense added successfully!");
    //         return true;
    //     }
    //     else if(type === "Income"){
    //         alert("Income added successfully!");
    //         return true;
    //     }
    //     else if(type === "Transfer"){
    //         alert("Transfer added successfully!");
    //         return true;
    //     }
    //     else {
    //         alert("Error. Unknown Transaction Type");
    //         return true;
    //     }

    // }
    // else {
    //     alert("Please fillout all fields!");
    //     return false;
    // }
     
    //     if(type === "Transfer"){
    //         if(transfOrigin != null && transfDestination != null){

    //         }
    //     }
        
    


    // console.log(type);

    
}


// var isAdvancedUpload = function () {
//     var div = document.createElement('div');
//     return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div)) && 'FormData' in window && 'FileReader' in window;
// }();

// if (isAdvancedUpload) {

//     var droppedFiles = false;

//     $form.on('drag dragstart dragend dragover dragenter dragleave drop', function (e) {
//         e.preventDefault();
//         e.stopPropagation();
//     })
//         .on('dragover dragenter', function () {
//             $form.addClass('is-dragover');
//         })
//         .on('dragleave dragend drop', function () {
//             $form.removeClass('is-dragover');
//         })
//         .on('drop', function (e) {
//             droppedFiles = e.originalEvent.dataTransfer.files;
//         });

// }

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
        event.stopPropagation();
        window.location.href = "AccountOverview?uuid=" + uuid;
    })
})




