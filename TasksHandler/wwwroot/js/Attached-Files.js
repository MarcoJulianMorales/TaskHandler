let inputTaskFile = document.getElementById('TaskFile');

function ClickAttachFile(){
    inputTaskFile.click();
}

async function SelectTaskFile(event) {
    const files = event.target.files;
    const filesArray = Array.from(files);

    const idTask = TaskEditVM.id;
    const formData = new FormData();
    for (var i = 0; i < filesArray.length; i++) {
        formData.append("files", filesArray[i]);
    }

    const response = await fetch(`${urlFiles}/${idTask}`, {
        body: formData,
        method: 'POST'
    });

    if (!response.ok) {
        HandleErrorApi(response);
        return;
    }

    const json = await response.json();
    console.log(json);

    inputTaskFile.value = null;
}