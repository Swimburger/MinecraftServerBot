
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Resources;

namespace MinecraftServerBot;

public class MinecraftServer
{
    private readonly ArmClient armClient;
    private readonly string resourceGroupName;
    private readonly string virtualMachineName;

    public MinecraftServer(
        ArmClient armClient,
        IConfiguration configuration
    )
    {
        this.armClient = armClient;
        this.resourceGroupName = configuration["Minecraft:ResourceGroupName"];
        this.virtualMachineName = configuration["Minecraft:VirtualMachineName"];
    }

    public async Task StartAsync()
    {
        VirtualMachineResource virtualMachine = await GetMinecraftVirtualMachine();
        await virtualMachine.PowerOnAsync(WaitUntil.Completed);
    }

    public async Task StopAsync()
    {
        VirtualMachineResource virtualMachine = await GetMinecraftVirtualMachine();
        await virtualMachine.DeallocateAsync(WaitUntil.Completed);
    }

    public async Task RestartAsync()
    {
        VirtualMachineResource virtualMachine = await GetMinecraftVirtualMachine();
        await virtualMachine.RestartAsync(WaitUntil.Completed);
    }

    private static readonly Dictionary<string, string> powerStateToStatusMap = new(){
        {"PowerState/deallocated", "off"},
        {"PowerState/deallocating", "turning off"},
        {"PowerState/running", "on"},
        {"PowerState/starting", "turning on"},
        {"PowerState/stopped", "off"},
        {"PowerState/stopping", "turning off"},
        {"PowerState/unknown", "unknown"},
        {string.Empty, "unknown"},
    };

    public async Task<string> GetStatus()
    {
        VirtualMachineResource virtualMachine = await GetMinecraftVirtualMachine();
        VirtualMachineInstanceView instanceView = await virtualMachine.InstanceViewAsync();
        var vmPowerStateCode = instanceView.Statuses
            .SingleOrDefault(s => s.Code.StartsWith("PowerState"))
            ?.Code ?? string.Empty;

        return powerStateToStatusMap[vmPowerStateCode];
    }

    public async Task<VirtualMachineResource> GetMinecraftVirtualMachine()
    {
        SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();
        ResourceGroupResource resourceGroup = await subscription.GetResourceGroupAsync(resourceGroupName);
        return await resourceGroup.GetVirtualMachineAsync(virtualMachineName);
    }
}