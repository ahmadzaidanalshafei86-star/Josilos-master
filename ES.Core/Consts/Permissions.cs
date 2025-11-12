using ES.Core.Enums;

namespace ES.Core.Consts
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsList(string moduleName)
        {
            return new List<string>
            {
                $"Permissions.{moduleName}.Create",
                $"Permissions.{moduleName}.Read",
                $"Permissions.{moduleName}.Update",
                $"Permissions.{moduleName}.Delete"
            };
        }

        public static List<string> GenerateAllPermissions()
        {
            var allPermissions = new List<string>();

            var modules = Enum.GetValues(typeof(Modules));

            foreach (var module in modules)
                allPermissions.AddRange(GeneratePermissionsList(module.ToString()!));

            return allPermissions;

        }


        public static class Roles
        {
            public const string Create = "Permissions.Roles.Create";
            public const string Read = "Permissions.Roles.Read";
            public const string Update = "Permissions.Roles.Update";
            public const string Delete = "Permissions.Roles.Delete";
        }
        public static class Users
        {
            public const string Create = "Permissions.Users.Create";
            public const string Read = "Permissions.Users.Read";
            public const string Update = "Permissions.Users.Update";
            public const string Delete = "Permissions.Users.Delete";
        }
        public static class Materials
        {
            public const string Create = "Permissions.Materials.Create";
            public const string Read = "Permissions.Materials.Read";
            public const string Update = "Permissions.Materials.Update";
            public const string Delete = "Permissions.Materials.Delete";
        }


        public static class Tenders
        {
            public const string Create = "Permissions.Tenders.Create";
            public const string Read = "Permissions.Tenders.Read";
            public const string Update = "Permissions.Tenders.Update";
            public const string Delete = "Permissions.Tenders.Delete";
        }
        public static class Categories
        {
            public const string Create = "Permissions.Categories.Create";
            public const string Read = "Permissions.Categories.Read";
            public const string Update = "Permissions.Categories.Update";
            public const string Delete = "Permissions.Categories.Delete";
        }

        public static class Pages
        {
            public const string Create = "Permissions.Pages.Create";
            public const string Read = "Permissions.Pages.Read";
            public const string Update = "Permissions.Pages.Update";
            public const string Delete = "Permissions.Pages.Delete";
        }

        public static class Documents
        {
            public const string Create = "Permissions.Documents.Create";
            public const string Read = "Permissions.Documents.Read";
            public const string Update = "Permissions.Documents.Update";
            public const string Delete = "Permissions.Documents.Delete";
        }

        public static class MenuManagment
        {
            public const string Create = "Permissions.MenuManagment.Create";
            public const string Read = "Permissions.MenuManagment.Read";
            public const string Update = "Permissions.MenuManagment.Update";
            public const string Delete = "Permissions.MenuManagment.Delete";
        }

        public static class MediaManagment
        {
            public const string Create = "Permissions.MediaManagment.Create";
            public const string Read = "Permissions.MediaManagment.Read";
            public const string Update = "Permissions.MediaManagment.Update";
            public const string Delete = "Permissions.MediaManagment.Delete";
        }

        public static class Forms
        {
            public const string Create = "Permissions.Forms.Create";
            public const string Read = "Permissions.Forms.Read";
            public const string Update = "Permissions.Forms.Update";
            public const string Delete = "Permissions.Forms.Delete";
        }

        public static class ProductCategories
        {
            public const string Create = "Permissions.ProductCategories.Create";
            public const string Read = "Permissions.ProductCategories.Read";
            public const string Update = "Permissions.ProductCategories.Update";
            public const string Delete = "Permissions.ProductCategories.Delete";
        }

        public static class Brands
        {
            public const string Create = "Permissions.Brands.Create";
            public const string Read = "Permissions.Brands.Read";
            public const string Update = "Permissions.Brands.Update";
            public const string Delete = "Permissions.Brands.Delete";
        }

        public static class ProductAttributes
        {
            public const string Create = "Permissions.ProductAttributes.Create";
            public const string Read = "Permissions.ProductAttributes.Read";
            public const string Update = "Permissions.ProductAttributes.Update";
            public const string Delete = "Permissions.ProductAttributes.Delete";
        }

        public static class Products
        {
            public const string Create = "Permissions.Products.Create";
            public const string Read = "Permissions.Products.Read";
            public const string Update = "Permissions.Products.Update";
            public const string Delete = "Permissions.Products.Delete";
        }

        public static class Careers
        {
            public const string Create = "Permissions.Careers.Create";
            public const string Read = "Permissions.Careers.Read";
            public const string Update = "Permissions.Careers.Update";
            public const string Delete = "Permissions.Careers.Delete";
        }

        public static class SocialMediaLinks
        {
            public const string Create = "Permissions.SocialMediaLinks.Create";
            public const string Read = "Permissions.SocialMediaLinks.Read";
            public const string Update = "Permissions.SocialMediaLinks.Update";
            public const string Delete = "Permissions.SocialMediaLinks.Delete";
        }

        public static class Productlabels
        {
            public const string Create = "Permissions.ProductLabels.Create";
            public const string Read = "Permissions.ProductLabels.Read";
            public const string Update = "Permissions.ProductLabels.Update";
            public const string Delete = "Permissions.ProductLabels.Delete";
        }

    }
}
