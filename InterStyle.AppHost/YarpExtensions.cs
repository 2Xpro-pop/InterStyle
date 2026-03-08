using Aspire.Hosting.Yarp;
using Aspire.Hosting.Yarp.Transforms;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.AppHost;

public static class YarpExtensions
{
    public static IResourceBuilder<YarpResource> ConfigureInterStyleRoutes(
        this IResourceBuilder<YarpResource> builder,
        IResourceBuilder<ProjectResource> leadsApi,
        IResourceBuilder<ProjectResource> reviewsApi,
        IResourceBuilder<ProjectResource> curtainsApi,
        IResourceBuilder<ProjectResource> imageApi,
        IResourceBuilder<ProjectResource> identityApi,
        IResourceBuilder<ProjectResource> adminPanel
        )
    {
        return builder.WithConfiguration(yarp =>
        {
            var leadsCluster = yarp.AddCluster(leadsApi);
            var reviewsCluster = yarp.AddCluster(reviewsApi);
            var curtainsCluster = yarp.AddCluster(curtainsApi);
            var imageCluster = yarp.AddCluster(imageApi);
            var identityCluster = yarp.AddCluster(identityApi);
            var adminPanelCluster = yarp.AddCluster(adminPanel);

            yarp.AddRoute("/api/leads", leadsCluster);
            yarp.AddRoute("/api/leads/{*any}", leadsCluster);

            yarp.AddRoute("/api/reviews", reviewsCluster);
            yarp.AddRoute("/api/reviews/{*any}", reviewsCluster);

            yarp.AddRoute("/api/curtains", curtainsCluster);
            yarp.AddRoute("/api/curtains/{*any}", curtainsCluster);

            yarp.AddRoute("/api/images", imageCluster);
            yarp.AddRoute("/api/images/{*any}", imageCluster);

            yarp.AddRoute("/api/identity", identityCluster);
            yarp.AddRoute("/api/identity/{*any}", identityCluster);

            yarp.AddRoute("/.well-known/{*any}", identityCluster);

            yarp.AddRoute("/admin", adminPanelCluster)
                .WithTransformPathRemovePrefix("/admin");

            yarp.AddRoute("/admin/{*any}", adminPanelCluster)
                .WithTransformPathRemovePrefix("/admin");
        });
    }
}