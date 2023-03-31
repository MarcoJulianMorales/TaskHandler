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
    prepareAttachedFiles(json);

    inputTaskFile.value = null;
}

function prepareAttachedFiles(attachedFiles) {
    attachedFiles.forEach(attachedFile => {
        let creationDate = attachedFile.creationDate;
        if (attachedFile.creationDate.indexOf('Z') === -1) {
            creationDate += 'Z';
        }

        const creationDateDT = new Date(creationDate);
        attachedFile.published = creationDateDT.toLocaleString();

        TaskEditVM.attachedFiles.push(
            new attachedFileVM({ ...attachedFile, editMode: false }));
    });
}

let prevtitleAttachedFile;
function ClickTitleAttachedFile(attachedFile) {
    attachedFile.editMode(true);
    prevtitleAttachedFile = attachedFile.title();
    $("[name='txtTitleAttachedFile']:visible").focus();
}

async function HandleFocusoutAttachedFile(attachedFile) {
    attachedFile.editMode(false);
    const idTask = attachedFile.id;

    if (!attachedFile.title()) {
        attachedFile.title(prevtitleAttachedFile);
    }

    if (attachedFile.title() === prevtitleAttachedFile) {
        return;
    }

    const data = JSON.stringify(attachedFile.title());

    const response = await fetch(`${urlFiles}/${idTask}`, {
        body: data,
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
            }
    });

    if (!response.ok) {
        HandleErrorApi(response);
    }
}

function ClickDeleteAttachedFile(attachedFile) {
    EditTaskModalBootstrap.hide();

    confirmAction({
        callBackAccept: () => {
            deleteAttachedFile(attachedFile);
            EditTaskModalBootstrap.show();
        },
        callBackCancel: () => {
            EditTaskModalBootstrap.show();
        },
        title: 'Delete attached file?'
        })
}

async function deleteAttachedFile(attachedFile) {
    const response = await fetch(`${urlFiles}/${attachedFile.id}`, {
        method: 'DELETE'
    });

    if (!response.ok) {
        HandleErrorApi(response);
        //return;
    }

    TaskEditVM.attachedFiles.remove(function (item) { return item.id == attachedFile.id });
}

function ClickDownloadAttachedFile(attachedFile) {
    downloadFile(attachedFile.url, attachedFile.title());
}