function AddStepHandle() {
    const indice = TaskEditVM.steps().findIndex(p => p.isNew());
    if (indice !== -1) {
        return; 
    }
    TaskEditVM.steps.push(new StepViewModel({ editMode: true, done: false }));
    $("[name=txtStepDescription]:visible").focus();
}

function CancelStep(step) {
    if (step.isNew()) {
        TaskEditVM.steps.pop();
    } else {
        step.editMode(false);
        step.description(step.prevDescription);
    }
}

async function SaveStep(step) {
    step.editMode(false);
    const isNew = step.isNew();
    const idTask = TaskEditVM.id;
    const data = getStepPetitionBody(step);

    const description = step.description();

    if (!description) {
        step.description(step.prevDescription);

        if (isNew) {
            TaskEditVM.steps.pop();
        }
        return;
    }

    if (isNew) {
        await insertStep(step, data, idTask);
    } else {
        UpdateStep(data, step.id());
    }
}

async function insertStep(step, data, idTask) {
    const response = await fetch(`${urlSteps}/${idTask}`, {
        body: data,
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {
        const json = await response.json();
        step.id(json.id);

        const task = getEditTask();
        task.totalSteps(task.totalSteps() + 1);

        if (step.done()) {
            task.doneSteps(task.doneSteps() + 1);
        }

    } else {
        HandleErrorApi(response)
    }
}

function getStepPetitionBody(step) {
    return JSON.stringify({
        description: step.description(),
        done: step.done()
    });
}

function EditStepDescription(step) {
    step.editMode(true);
    step.prevDescription = step.description();
    $("[name=txtStepDescription]:visible").focus();
}

async function UpdateStep(data, id) {
    const response = await fetch(`${urlSteps}/${id}`, {
        body: data,
        method: "PUT",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        HandleErrorApi(response);
    }
}

function ClickCheckBoxStep(step) {
    if (step.isNew()) {
        return true;
    }

    const data = getStepPetitionBody(step);
    UpdateStep(data, step.id());

    const task = getEditTask();

    let currentDoneSteps = task.doneSteps();

    if (step.done()) {
        currentDoneSteps++;
    } else {
        currentDoneSteps--;
    }

    task.doneSteps(currentDoneSteps);

    return true;
}

function ClickDeleteStep(step) {
    EditTaskModalBootstrap.hide();
    confirmAction({
        callBackAccept: () => {
            DeteleStep(step);
            EditTaskModalBootstrap.show();
        },
        callBackCancel: () => {
            EditTaskModalBootstrap.show();
        },
        title: `Delete Step?`
        })
}

async function DeteleStep(step) {
    const response = await fetch(`${urlSteps}/${step.id()}`, {
        method: "DELETE"
    });

    if (!response.ok) {
        HandleErrorApi(response);
        return;
    }

    TaskEditVM.steps.remove(function (item) { return item.id() == step.id() });

    const task = getEditTask();

    task.totalSteps(task.totalSteps() - 1);

    if (step.done()) {
        task.doneSteps(task.doneSteps() - 1);
    }
}