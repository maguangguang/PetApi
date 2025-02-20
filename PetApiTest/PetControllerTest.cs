using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using PetApi;
using Xunit;

namespace PetApiTest
{
    public class PetControllerTest
    {
        [Fact]
        public async void Should_add_new_pet_successfully()
        {
            var client = await ResetContextAndGetHttpClient();

            var pet = new Pet(name: "Baymax", type: "dog", color: "white", price: 1000);
            var stringContent = SerializeToJsonString(pet);

            var response = await client.PostAsync("api/pets", stringContent);

            response.EnsureSuccessStatusCode();
            var savedPet = await DeserializeToType<Pet>(response);
            Assert.Equal(pet, savedPet);
        }

        [Fact]
        public async void Should_get_all_pets_successfully()
        {
            var client = await ResetContextAndGetHttpClient();

            var petBaymax = new Pet(name: "Baymax", type: "dog", color: "white", price: 1000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBaymax));
            var petJinMao = new Pet(name: "JinMao", type: "dog", color: "white", price: 5000);
            await client.PostAsync("api/pets", SerializeToJsonString(petJinMao));

            var response = await client.GetAsync("/api/pets");

            response.EnsureSuccessStatusCode();
            var allPets = await DeserializeToType<List<Pet>>(response);
            Assert.Equal(new List<Pet>() { petBaymax, petJinMao }, allPets);
        }

        [Fact]
        public async void Should_find_pet_by_name_successfully()
        {
            var client = await ResetContextAndGetHttpClient();

            var petBaymax = new Pet(name: "Baymax", type: "dog", color: "white", price: 1000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBaymax));
            var petJinMao = new Pet(name: "JinMao", type: "dog", color: "white", price: 5000);
            await client.PostAsync("api/pets", SerializeToJsonString(petJinMao));

            var response = await client.GetAsync("/api/pets/Baymax");

            response.EnsureSuccessStatusCode();
            var foundPet = await DeserializeToType<Pet>(response);
            Assert.Equal(petBaymax, foundPet);
        }

        [Fact]
        public async void Should_delete_pet_by_name_successfully()
        {
            var client = await ResetContextAndGetHttpClient();

            var petBaymax = new Pet(name: "Baymax", type: "dog", color: "white", price: 1000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBaymax));
            var petJinMao = new Pet(name: "JinMao", type: "dog", color: "white", price: 5000);
            await client.PostAsync("api/pets", SerializeToJsonString(petJinMao));

            await client.DeleteAsync("/api/pets/Baymax");
            var response = await client.GetAsync("/api/pets");

            response.EnsureSuccessStatusCode();
            var allPets = await DeserializeToType<List<Pet>>(response);
            Assert.Equal(new List<Pet>() { petJinMao }, allPets);
        }

        [Fact]
        public async void Should_modify_pet_info_by_name_successfully()
        {
            var client = await ResetContextAndGetHttpClient();

            var petBaymax = new Pet(name: "Baymax", type: "dog", color: "white", price: 1000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBaymax));

            var petModified = new Pet(name: "Baymax", type: "dog", color: "black", price: 2000);
            var response = await client.PutAsync("/api/pets/Baymax", SerializeToJsonString(petModified));

            response.EnsureSuccessStatusCode();
            var petAfterModified = await DeserializeToType<Pet>(response);
            Assert.Equal(petModified, petAfterModified);
        }

        [Fact]
        public async void Should_find_pets_by_type_successfully()
        {
            var client = await ResetContextAndGetHttpClient();

            var petBaymax = new Pet(name: "Baymax", type: "dog", color: "white", price: 1000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBaymax));
            var petJinMao = new Pet(name: "JinMao", type: "dog", color: "white", price: 5000);
            await client.PostAsync("api/pets", SerializeToJsonString(petJinMao));
            var petBuou = new Pet(name: "Buou", type: "cat", color: "gray", price: 3000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBuou));

            var response = await client.GetAsync("/api/pets?type=dog");

            response.EnsureSuccessStatusCode();
            var fountPets = await DeserializeToType<List<Pet>>(response);
            Assert.Equal(new List<Pet>() { petBaymax, petJinMao }, fountPets);
        }

        [Fact]
        public async void Should_find_pets_by_price_range_successfully()
        {
            var client = await ResetContextAndGetHttpClient();

            var petBaymax = new Pet(name: "Baymax", type: "dog", color: "white", price: 1000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBaymax));
            var petJinMao = new Pet(name: "JinMao", type: "dog", color: "white", price: 5000);
            await client.PostAsync("api/pets", SerializeToJsonString(petJinMao));
            var petBuou = new Pet(name: "Buou", type: "cat", color: "gray", price: 3000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBuou));

            var response = await client.GetAsync("/api/pets?priceFrom=1000&priceTo=3000");

            response.EnsureSuccessStatusCode();
            var fountPets = await DeserializeToType<List<Pet>>(response);
            Assert.Equal(new List<Pet>() { petBaymax, petBuou }, fountPets);
        }

        [Fact]
        public async void Should_find_pets_by_color_successfully()
        {
            var client = await ResetContextAndGetHttpClient();

            var petBaymax = new Pet(name: "Baymax", type: "dog", color: "white", price: 1000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBaymax));
            var petJinMao = new Pet(name: "JinMao", type: "dog", color: "white", price: 5000);
            await client.PostAsync("api/pets", SerializeToJsonString(petJinMao));
            var petBuou = new Pet(name: "Buou", type: "cat", color: "gray", price: 3000);
            await client.PostAsync("api/pets", SerializeToJsonString(petBuou));

            var response = await client.GetAsync("/api/pets?color=white");

            response.EnsureSuccessStatusCode();
            var fountPets = await DeserializeToType<List<Pet>>(response);
            Assert.Equal(new List<Pet>() { petBaymax, petJinMao }, fountPets);
        }

        private static async Task<HttpClient> ResetContextAndGetHttpClient()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            var client = server.CreateClient();
            await ClearPets(client);
            return client;
        }

        private static async Task ClearPets(HttpClient client)
        {
            await client.DeleteAsync("/api/pets");
        }

        private static async Task<T> DeserializeToType<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        private static StringContent SerializeToJsonString(Pet pet)
        {
            return new StringContent(JsonConvert.SerializeObject(pet), Encoding.UTF8, "application/json");
        }
    }
}