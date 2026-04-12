function cancelarPedido(id)
{

    if (!confirm("¿Seguro que querés cancelar este pedido?")) return;

    fetch('/Pedido/Cancelar', {
    method: 'POST',
        headers:
        {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id: id })
    })
    .then(res => {
         if (!res.ok) throw new Error("Error al cancelar");
         return res.text();
     })
    .then(() => {
        alert("Pedido cancelado correctamente");
        location.reload();
    })
    .catch(err => {
        console.error(err);
        alert("Error al cancelar el pedido");
    });
}
