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