function addNewTask() {
    TasksListDTO.tasks.push(new TasksElementListDTO({ id: 0, Title: '' }));
    $("[name=Title-task]").last().focus();
}

async function focusoutTaskTitle(task) {
    const Title = task.title();
    if (!Title) {
        TasksListDTO.tasks.push(new TasksElementListDTO({ id: 0, title: '' }));
        $("[name=title-task]").last().focus();
    }
}

async function focusoutTaskTitle(task) {
    const title = task.Title();
    if (!title) {
        TasksListDTO.tasks.pop();
        return;
    }
    //task.id(1);
    const data = JSON.stringify(title);
    const response = await fetch(urlTasks, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {
        const json = await response.json();
        task.id(json.id);
    } else {
        HandleErrorApi(response);
    }
}

async function getTasks() {
    TasksListDTO.loading(true);

    const response = await fetch(urlTasks, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
            }
    })

    if (!response.ok) {
        HandleErrorApi(response);
        return;
    }

    const json = await response.json();
    TasksListDTO.tasks([]);

    json.forEach(valor => {
        TasksListDTO.tasks.push(new TasksElementListDTO(valor));
    });

    TasksListDTO.loading(false);
}

async function updateTasksOrder() {
    const ids = getTasksIds();
    await sendTasksIdsToBackend(ids);

    const sortedArray = TasksListDTO.tasks.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id.toString());
    });

    TasksListDTO.tasks([]);
    getTasks();
}

function getTasksIds() {
    const ids = $("[name=Title-task]").map(function () {
        return $(this).attr("data-id");
    }).get();

    return ids;
}

async function sendTasksIdsToBackend(ids) {
    var data = JSON.stringify(ids);
    await fetch(`${urlTasks}/sort`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

 async function TaskClickHandler(task) {
    if (task.isNew()) {
        return;
    }

    const response = await fetch(`${urlTasks}/${task.id()}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        HandleErrorApi(response);
        return;
    }

    const json = await response.json();
     console.log(json);

     TaskEditVM.id = json.id;
     TaskEditVM.title(json.Title);
     TaskEditVM.description(json.description);

}


$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await updateTasksOrder();
        }
        })
})