async function HandleErrorApi(response) {
    let errorMessage = '';

    if (response.status === 400) {
        errorMessage = await response.text();
    } else if (response.status === 404) {
        errorMessage = ResourceNotFound;
    } else {
        errorMessage = UnexpectedError;
    }

    showErrorMessage(errorMessage);
}

function showErrorMessage(message) {
    Swal.fire({
        icon: 'error',
        title: 'Error...',
        text: message
    });
}

function confirmAction({ callBackAccept, callBackCancel, title }) {
    Swal.fire({
        title: title || 'Are you sure?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes',
        focusConfirm: true
    }).then((result) => {
        if (result.isConfirmed) {
            callBackAccept();
        } else {
            //user clicked cancel
            callBackCancel();
        }
    })
}