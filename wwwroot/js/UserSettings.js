function editAccountWindow(event, button) {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('edit-fin-acc-window').classList.toggle("visible");

    const accNameForInp = button.getAttribute('data-acc-name');
    const accID = button.getAttribute('data-id');
    const accType = button.getAttribute('data-acc-type');


    document.getElementById('acc-name-inp').value = accNameForInp;
    document.getElementById('acc-ID').value = accID;
    document.getElementById('acc-Type').value = accType;
}

function closeEditMenu() {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('edit-fin-acc-window').classList.toggle("visible");
}


function toggleEdit(credFieldId, editBtnId) {

    const allFields = document.querySelectorAll('input[type="text"], input[type="tel"], input[type="password"]');
    const allButtons = document.querySelectorAll('.edit-btn');

    // Disable all fields and reset button texts
    allFields.forEach(field => field.disabled = true);
    allButtons.forEach(button => button.textContent = '🖊️');

    const credField = document.getElementById(credFieldId);
    const editBtn = document.getElementById(editBtnId);
    credField.disabled = !credField.disabled;
    if (credField.disabled) {
        editBtn.textContent = '🖊️'
    }
    else {
        editBtn.textContent = '❌'
    }
}


function togglePswrdChange() {
    document.getElementById('new-pswrd-container').classList.toggle("visible");
}


function toggleConfirmDeletion(event, button) {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('confirm-deletion-container').classList.toggle("visible");

    const accType = button.getAttribute('data-acc-type');
    const accID = button.getAttribute('data-acc-id');
    const accName = button.getAttribute('data-acc-name');

    console.log(accType, accID);

    document.getElementById('acc-type-data-field').value = accType;
    document.getElementById('acc-id-data-field').value = accID;
    document.getElementById('acc-name').innerHTML = accName;
}

function closeDeleteMenu() {
    document.getElementById('dark-overlay').classList.toggle("visible");
    document.getElementById('confirm-deletion-container').classList.toggle("visible");
}